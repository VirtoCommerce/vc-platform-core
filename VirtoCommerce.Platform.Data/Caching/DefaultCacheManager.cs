using System;
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

        public DefaultCacheManager(IPlatformMemoryCache cache)
        {
            _cache = cache;
            _cancellationTokenSource = new CancellationTokenSource();
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
                _cache.Set(key, data, cacheTime);
            }
        }

        public virtual void Remove(string key)
        {
            _cache.Remove(key);
        }

        public virtual void Clear()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = new CancellationTokenSource();
        }


    }
}
