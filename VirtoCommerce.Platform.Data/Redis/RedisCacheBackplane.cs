using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using StackExchange.Redis.Extensions.Core;
using VirtoCommerce.Platform.Core.Caching;

namespace VirtoCommerce.Platform.Data.Redis
{
    /// <summary>
    /// Implementation of the cache backplane using a Redis Pub/Sub channel.
    /// <para>
    /// Redis Pub/Sub is used to send messages to the redis server on any key change, cache clear, region
    /// clear or key remove operation.
    /// Every cache manager with the same configuration subscribes to the
    /// same channel and can react on those messages to keep other cache handles in sync with the 'master'.
    /// </para>
    /// </summary>
    /// <remarks>
    /// The cache manager must have at least one cache handle configured with <see cref="CacheHandleConfiguration.IsBackplaneSource"/> set to <c>true</c>.
    /// Usually this is the redis cache handle, if configured. It should be the distributed and bottom most cache handle.
    /// </remarks>
    public sealed class RedisCacheBackplane : ICacheBackplane, IDisposable
    {
        public static long MessagesReceived = 0;

        private const int HardLimit = 50000;
        private readonly string _channelName;
        private readonly byte[] _identifier;
        private readonly ILogger _logger;
        private readonly RedisConnectionManager _connection;
        private readonly Timer _timer;
        private HashSet<BackplaneMessage> _messages = new HashSet<BackplaneMessage>();
        private object _messageLock = new object();
        private int _skippedMessages = 0;
        private bool _sending = false;
        private CancellationTokenSource _source = new CancellationTokenSource();
        private bool loggedLimitWarningOnce = false;
        private readonly ISerializer _serializer;
        private readonly IPlatformMemoryCache _platformMemoryCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisCacheBackplane"/> class.
        /// </summary>
        /// <param name="configuration">The cache manager configuration.</param>
        /// <param name="logger">The logger factory</param>
        public RedisCacheBackplane(IConfiguration configuration, ILogger<RedisCacheBackplane> logger, ISerializer serializer, IPlatformMemoryCache platformMemoryCache)
        {
            _logger = logger;
            _serializer = serializer;
            _platformMemoryCache = platformMemoryCache;

            _channelName = "CacheManagerBackplane";
            _identifier = Encoding.UTF8.GetBytes(Guid.NewGuid().ToString());
            ConfigurationKey = "RedisConnection";

            var cfg = RedisConfigurations.GetConfiguration(configuration, ConfigurationKey);
            _connection = new RedisConnectionManager(cfg, logger);

            Subscribe();

            // adding additional timer based send message invoke (shouldn't do anything if there are no messages,
            // but in really rare race conditions, it might happen messages do not get send if SendMEssages only get invoked through "NotifyXyz"
            //_timer = new Timer(SendMessages, true, 1000, 1000);
        }

        public string ConfigurationKey { get; }


        /// <summary>
        /// Notifies other cache clients about a changed cache key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="action">The cache action.</param>
        public void NotifyChange(string key, CacheItemChangedEventAction action)
        {
            var message = new BackplaneMessage(_identifier, BackplaneAction.Changed, key, action);

            PublishMessage(message);
        }

        /// <summary>
        /// Notifies other cache clients about a changed cache key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="region">The region.</param>
        /// <param name="action">The cache action.</param>
        public void NotifyChange(string key, string region, CacheItemChangedEventAction action)
        {
            var message = new BackplaneMessage(_identifier, BackplaneAction.Changed, key, region, action);

            PublishMessage(message);
        }

        /// <summary>
        /// Notifies other cache clients about a cache clear.
        /// </summary>
        public void NotifyClear()
        {
            var message = new BackplaneMessage(_identifier, BackplaneAction.Clear);

            PublishMessage(message);
        }

        /// <summary>
        /// Notifies other cache clients about a cache clear region call.
        /// </summary>
        /// <param name="region">The region.</param>
        public void NotifyClearRegion(string region)
        {
            var message = new BackplaneMessage(_identifier, BackplaneAction.Clear)
            {
                Region = region
            };

            PublishMessage(message);
        }

        /// <summary>
        /// Notifies other cache clients about a removed cache key.
        /// </summary>
        /// <param name="key">The key.</param>
        public void NotifyRemove(string key)
        {
            var message = new BackplaneMessage(_identifier, BackplaneAction.Removed, key);

            PublishMessage(message);
        }

        /// <summary>
        /// Notifies other cache clients about a removed cache key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="region">The region.</param>
        public void NotifyRemove(string key, string region)
        {
            var message = new BackplaneMessage(_identifier, BackplaneAction.Removed, key, region);

            PublishMessage(message);
        }



        private void Publish(byte[] message)
        {
            _connection.Subscriber.Publish(_channelName, message);
        }

        private void PublishMessage(BackplaneMessage message)
        {
            lock (_messageLock)
            {
                if (message.Action == BackplaneAction.Clear)
                {
                    Interlocked.Exchange(ref _skippedMessages, _messages.Count);
                    _messages.Clear();
                }

                if (_messages.Count > HardLimit)
                {
                    if (!loggedLimitWarningOnce)
                    {
                        _logger.LogError("Exceeded hard limit of number of messages pooled to send through the backplane. Skipping new messages...");
                        loggedLimitWarningOnce = true;
                    }
                }
                else if (!_messages.Add(message))
                {
                    Interlocked.Increment(ref _skippedMessages);
                    if (_logger.IsEnabled(LogLevel.Trace))
                    {
                        _logger.LogTrace("Skipped duplicate message: {0}.", message);
                    }
                }

                SendMessages(null);
            }
        }

        private void SendMessages(object state)
        {
            if (_sending || _messages == null || _messages.Count == 0)
            {
                return;
            }

            Task.Factory.StartNew(
                async (obj) =>
                {
                    if (_sending || _messages == null || _messages.Count == 0)
                    {
                        return;
                    }

                    _sending = true;
                    if (state != null && state is bool boolState && boolState == true)
                    {
                        _logger.LogInformation($"Backplane is sending {_messages.Count} messages triggered by timer.");
                    }
#if !NET40
                    await Task.Delay(10).ConfigureAwait(false);
#endif
                    byte[] msgs = null;
                    lock (_messageLock)
                    {
                        if (_messages != null && _messages.Count > 0)
                        {
                            msgs = _serializer.Serialize(_messages.ToArray());

                            if (_logger.IsEnabled(LogLevel.Debug))
                            {
                                _logger.LogDebug("Backplane is sending {0} messages ({1} skipped).", _messages.Count, _skippedMessages);
                            }

                            try
                            {
                                if (msgs != null)
                                {
                                    Publish(msgs);
                                    //Interlocked.Increment(ref SentChunks);
                                    //Interlocked.Add(ref MessagesSent, _messages.Count);
                                    _skippedMessages = 0;

                                    // clearing up only after successfully sending. Basically retrying...
                                    _messages.Clear();

                                    // reset log limmiter because we just send stuff
                                    loggedLimitWarningOnce = false;
                                }
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "Error occurred sending backplane messages.");
                            }
                        }

                        _sending = false;
                    }
                },
                this,
                _source.Token,
                TaskCreationOptions.DenyChildAttach,
                TaskScheduler.Default)
                .ConfigureAwait(false);
        }

        private void Subscribe()
        {
            _connection.Subscriber.Subscribe(
                _channelName,
                (channel, msg) =>
                {
                    try
                    {
                        var messages = _serializer.Deserialize<BackplaneMessage[]>(msg);

                        if (!messages.Any())
                        {
                            // no messages for this instance
                            return;
                        }

                        Interlocked.Add(ref MessagesReceived, messages.Length);
                        if (_logger.IsEnabled(LogLevel.Information))
                        {
                            _logger.LogInformation("Backplane got notified with {0} new messages.", messages.Length);
                        }

                        foreach (var message in messages)
                        {
                            switch (message.Action)
                            {
                                case BackplaneAction.Clear:
                                    TriggerCleared();
                                    break;

                                case BackplaneAction.ClearRegion:
                                    TriggerClearedRegion(message.Region);
                                    break;

                                case BackplaneAction.Changed:
                                    if (string.IsNullOrWhiteSpace(message.Region))
                                    {
                                        TriggerChanged(message.Key, message.ChangeAction);
                                    }
                                    else
                                    {
                                        TriggerChanged(message.Key, message.Region, message.ChangeAction);
                                    }
                                    break;

                                case BackplaneAction.Removed:
                                    if (string.IsNullOrWhiteSpace(message.Region))
                                    {
                                        TriggerRemoved(message.Key);
                                    }
                                    else
                                    {
                                        TriggerRemoved(message.Key, message.Region);
                                    }
                                    break;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Error reading backplane message(s)");
                    }
                },
                CommandFlags.FireAndForget);
        }

        private void TriggerCleared()
        {

        }

        private void TriggerClearedRegion(string region)
        {

        }

        private void TriggerChanged(string key, CacheItemChangedEventAction action)
        {
            _platformMemoryCache.Remove(key);
        }

        private void TriggerChanged(string key, string region, CacheItemChangedEventAction action)
        {

        }

        private void TriggerRemoved(string key)
        {

        }

        private void TriggerRemoved(string key, string region)
        {

        }

        /// <summary>
        /// Finalizes an instance of the <see cref="RedisCacheBackplane"/> class.
        /// </summary>
        ~RedisCacheBackplane()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }


        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="managed">
        /// <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release
        /// only unmanaged resources.
        /// </param>
        private void Dispose(bool managed)
        {
            if (managed)
            {
                try
                {
                    _source.Cancel();
                    _connection.Subscriber.Unsubscribe(_channelName);
                    _timer.Dispose();
                }
                catch
                {
                }
            }
        }
    }
}

