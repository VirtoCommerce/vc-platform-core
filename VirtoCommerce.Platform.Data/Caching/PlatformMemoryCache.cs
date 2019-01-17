using System;
using Microsoft.Extensions.Caching.Memory;
using VirtoCommerce.Platform.Core;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Settings;

namespace VirtoCommerce.Platform.Data.Caching
{
    public class PlatformMemoryCache : IPlatformMemoryCache
    {
        private readonly ISettingsManager _settingManager;
        private readonly IMemoryCache _memoryCache;
        private static TimeSpan? _absoluteExpiration;
        private static bool? _cacheEnabled;
        private bool _disposed;
        private static object _lockObject = new object();

        public PlatformMemoryCache(IMemoryCache memoryCache, ISettingsManager settingManager)
        {
            _memoryCache = memoryCache;
            _settingManager = settingManager;
        }

        public ICacheEntry CreateEntry(object key)
        {
            var result = _memoryCache.CreateEntry(key);
            if (result != null)
            {
                var absoluteExpiration = CacheEnabled ? AbsoluteExpiration : TimeSpan.FromTicks(1);
                result.SetAbsoluteExpiration(absoluteExpiration);
            }

            return result;
        }

        public virtual void Remove(object key)
        {
            _memoryCache.Remove(key);
        }

        public bool TryGetValue(object key, out object value)
        {
            return _memoryCache.TryGetValue(key, out value);
        }

        public void Set(string key, object data, int? cacheTime = null)
        {
            var absoluteExpiration = CacheEnabled ? AbsoluteExpiration : TimeSpan.FromTicks(1);
            _memoryCache.Set(key, data, absoluteExpiration);
        }

        protected TimeSpan AbsoluteExpiration
        {
            get
            {
                if (_absoluteExpiration == null)
                {
                    lock (_lockObject)
                    {
                        if (_absoluteExpiration == null)
                        {
                            _absoluteExpiration = TimeSpan.Parse(_settingManager.GetValue(PlatformConstants.Settings.Cache.AbsoluteExpiration.Name, (string)PlatformConstants.Settings.Cache.AbsoluteExpiration.DefaultValue));
                        }
                    }
                }
                return _absoluteExpiration.Value;
            }
        }

        protected bool CacheEnabled
        {
            get
            {
                if (_cacheEnabled == null)
                {
                    lock (_lockObject)
                    {
                        if (_cacheEnabled == null)
                        {
                            _cacheEnabled = Convert.ToBoolean(_settingManager.GetValue(PlatformConstants.Settings.Cache.CacheEnabled.Name, true));
                        }
                    }
                }
                return _cacheEnabled.Value;
            }
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
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    GC.SuppressFinalize(this);
                }

                _disposed = true;
            }
        }
    }
}
