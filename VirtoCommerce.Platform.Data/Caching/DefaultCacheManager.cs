using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using VirtoCommerce.Platform.Core.Caching;

namespace VirtoCommerce.Platform.Data.Caching
{
    public class DefaultCacheManager : ICacheManager
    {
        private readonly IPlatformMemoryCache _cache;
        protected CancellationTokenSource _cancellationTokenSource;
        protected readonly ConcurrentDictionary<string, bool> _allKeys;

        public DefaultCacheManager(IPlatformMemoryCache cache)
        {
            _cache = cache;
            _cancellationTokenSource = new CancellationTokenSource();
            _allKeys = new ConcurrentDictionary<string, bool>();
        }

        /// <summary>
        /// Add key to dictionary
        /// </summary>
        /// <param name="key">Key of cached item</param>
        /// <returns>Itself key</returns>
        protected string AddKey(string key)
        {
            _allKeys.TryAdd(key, true);
            return key;
        }

        /// <summary>
        /// Remove key from dictionary
        /// </summary>
        /// <param name="key">Key of cached item</param>
        /// <returns>Itself key</returns>
        protected string RemoveKey(string key)
        {
            TryRemoveKey(key);
            return key;
        }

        /// <summary>
        /// Try to remove a key from dictionary, or mark a key as not existing in cache
        /// </summary>
        /// <param name="key">Key of cached item</param>
        protected void TryRemoveKey(string key)
        {
            //try to remove key from dictionary
            if (!_allKeys.TryRemove(key, out _))
                //if not possible to remove key from dictionary, then try to mark key as not existing in cache
                _allKeys.TryUpdate(key, false, true);
        }

        public virtual T Get<T>(string key, Func<T> acquire, int? cacheTime = null)
        {
            return GetAsync(key, () => Task.Run(acquire), cacheTime).GetAwaiter().GetResult();
        }

        public virtual async Task<T> GetAsync<T>(string key, Func<Task<T>> factory, int? cacheTime = null)
        {
            if (_cache.TryGetValue(key, out T value))
                return value;

            var result = await factory();

            Set(key, result, cacheTime);

            return result;
        }

        public virtual void Set(string key, object data, int? cacheTime = null)
        {
            if (data != null)
            {
                _cache.Set(AddKey(key), data, cacheTime);
            }
        }

        public virtual void Remove(string key)
        {
            _cache.Remove(RemoveKey(key));
        }

        public virtual Task RemoveAsync(string key)
        {
            Remove(RemoveKey(key));

            return Task.CompletedTask;
        }

        public virtual void RemoveByPattern(string pattern)
        {
            var regex = new Regex(pattern, RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase);
            var matchesKeys = _allKeys.Where(p => p.Value).Select(p => p.Key).Where(key => regex.IsMatch(key)).ToList();

            //remove matching values
            foreach (var key in matchesKeys)
            {
                Remove(RemoveKey(key));
            }
        }

        public virtual Task RemoveByPatternAsync(string pattern)
        {
            var regex = new Regex(pattern, RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase);
            var matchesKeys = _allKeys.Where(p => p.Value).Select(p => p.Key).Where(key => regex.IsMatch(key)).ToList();

            //remove matching values
            foreach (var key in matchesKeys)
            {
                Remove(RemoveKey(key));
            }

            return Task.CompletedTask;
        }

        public virtual void Clear()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = new CancellationTokenSource();
        }


    }
}
