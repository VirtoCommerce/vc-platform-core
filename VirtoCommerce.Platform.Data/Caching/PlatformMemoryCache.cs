using System;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using VirtoCommerce.Platform.Core;
using VirtoCommerce.Platform.Core.Caching;

namespace VirtoCommerce.Platform.Data.Caching
{
    public class PlatformMemoryCache : IPlatformMemoryCache
    {
        private readonly PlatformOptions _platformOptions;
        private readonly IMemoryCache _memoryCache;
        private bool _disposed;

        public PlatformMemoryCache(IMemoryCache memoryCache, IOptions<PlatformOptions> options)
        {
            _memoryCache = memoryCache;
            _platformOptions = options.Value;
        }

        protected bool CacheEnabled => _platformOptions.CacheEnabled;
        protected TimeSpan? AbsoluteExpiration => _platformOptions.CacheAbsoluteExpiration;
        protected TimeSpan? SlidingExpiration => _platformOptions.CacheSlidingExpiration;

        public ICacheEntry CreateEntry(object key)
        {
            var result = _memoryCache.CreateEntry(key);
            if (result != null)
            {
                var options = GetDefaultCacheEntryOptions();
                result.SetOptions(options);
            }
            return result;
        }

        public void Remove(object key)
        {
            _memoryCache.Remove(key);
        }

        public bool TryGetValue(object key, out object value)
        {
            return _memoryCache.TryGetValue(key, out value);
        }

        private MemoryCacheEntryOptions GetDefaultCacheEntryOptions()
        {
            var result = new MemoryCacheEntryOptions();

            if (!CacheEnabled)
            {
                result.AbsoluteExpirationRelativeToNow = TimeSpan.FromTicks(1);
            }
            else
            {
                if (AbsoluteExpiration != null)
                {
                    result.AbsoluteExpirationRelativeToNow = AbsoluteExpiration;
                }
                else if (SlidingExpiration != null)
                {
                    result.SlidingExpiration = SlidingExpiration;
                }
            }
            return result;
        }

        /// <summary>
        /// Cleans up the background collection events.
        /// </summary>
        ~PlatformMemoryCache()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SupressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _memoryCache.Dispose();
                }
                _disposed = true;
            }
        }
    }
}
