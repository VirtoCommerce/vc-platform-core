using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using VirtoCommerce.Platform.Core;
using VirtoCommerce.Platform.Core.Caching;

namespace VirtoCommerce.Platform.Caching
{
    public class PlatformMemoryCache : IPlatformMemoryCache
    {
        private readonly CachingOptions _cachingOptions;
        private readonly IMemoryCache _memoryCache;
        private bool _disposed;
        protected readonly ConcurrentDictionary<object, bool> _allKeys = new ConcurrentDictionary<object, bool>();

        public PlatformMemoryCache(IMemoryCache memoryCache, IOptions<CachingOptions> options)
        {
            _memoryCache = memoryCache;
            _cachingOptions = options.Value;
        }

        protected bool CacheEnabled => _cachingOptions.CacheEnabled;
        protected TimeSpan? AbsoluteExpiration => _cachingOptions.CacheAbsoluteExpiration;
        protected TimeSpan? SlidingExpiration => _cachingOptions.CacheSlidingExpiration;

        public virtual ICacheEntry CreateEntry(object key)
        {
            var result = _memoryCache.CreateEntry(AddKey(key));
            if (result != null)
            {
                var options = GetDefaultCacheEntryOptions();
                result.SetOptions(options);
            }
            return result;
        }

        public virtual void Remove(object key)
        {
            RemoveKey(key);
            _memoryCache.Remove(key);
        }

        public virtual void RemoveByPattern(string pattern)
        {
            var regex = new Regex(pattern, RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase);
            var matchesKeys = _allKeys.Where(p => p.Value).Select(p => p.Key).Where(key => regex.IsMatch(key.ToString())).ToList();

            foreach (var key in matchesKeys)
            {
                Remove(key);
            }
        }

        public virtual bool TryGetValue(object key, out object value)
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

        public virtual void Dispose()
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


        protected object AddKey(object key)
        {
            _allKeys.TryAdd(key, true);
            return key;
        }

        protected void RemoveKey(object key)
        {
            TryRemoveKey(key.ToString());
        }

        protected void TryRemoveKey(string key)
        {
            if (!_allKeys.TryRemove(key, out _))
            {
                _allKeys.TryUpdate(key, false, true);
            }
        }
    }
}
