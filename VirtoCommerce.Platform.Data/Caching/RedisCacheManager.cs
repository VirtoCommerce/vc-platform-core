using System.Threading;
using VirtoCommerce.Platform.Core.Caching;

namespace VirtoCommerce.Platform.Data.Caching
{
    public class RedisCacheManager : DefaultCacheManager, ICacheManager
    {
        private readonly ICacheBackplane _cacheBackplane;

        public RedisCacheManager(IPlatformMemoryCache cache, ICacheBackplane cacheBackplane) : base(cache)
        {
            _cacheBackplane = cacheBackplane;
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public override void Set(string key, object data, int? cacheTime = null)
        {
            if (data != null)
            {
                //base.Set(key, data, cacheTime);
                _cacheBackplane.NotifyChangeAsync(key, CacheItemChangedEventAction.Add);
            }
        }

        public override void Remove(string key)
        {
            //base.Remove(key);
            _cacheBackplane.NotifyRemoveAsync(key);
        }
    }
}
