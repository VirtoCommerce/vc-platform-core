using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using StackExchange.Redis.Extensions.Core;
using VirtoCommerce.Platform.Caching;
using VirtoCommerce.Platform.Core;
using VirtoCommerce.Platform.Core.Caching;

namespace VirtoCommerce.Platform.Redis
{
    public class RedisPlatformMemoryCache : PlatformMemoryCache, IPlatformMemoryCache
    {
        private readonly ISubscriber _bus;
        private readonly RedisCachingOptions _redisCachingOptions;
        private readonly ISerializer _serializer;

        public RedisPlatformMemoryCache(IMemoryCache memoryCache, IOptions<CachingOptions> options
            , ISubscriber bus
            , IOptions<RedisCachingOptions> redisCachingOptions
            , ISerializer serializer
            ) : base(memoryCache, options)
        {
            _bus = bus;
            _redisCachingOptions = redisCachingOptions.Value;
            _bus.Subscribe(_redisCachingOptions.ChannelName, OnMessage);
            _serializer = serializer;
        }

        public override ICacheEntry CreateEntry(object key)
        {
            return base.CreateEntry(key);
        }

        public override void Remove(object key)
        {
            _bus.Publish(_redisCachingOptions.ChannelName, _serializer.Serialize(key));
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        private void OnMessage(RedisChannel channel, RedisValue value)
        {
            var message = _serializer.Deserialize<object>(value);
            base.Remove(message);
        }
    }
}
