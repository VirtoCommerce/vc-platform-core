using System.Threading.Tasks;
using VirtoCommerce.Platform.Core.Caching;

namespace VirtoCommerce.Platform.Data.Caching
{
    public class RedisCacheManager : DefaultCacheManager, ICacheManager
    {
        private readonly ICacheBackplane _cacheBackplane;

        public RedisCacheManager(IPlatformMemoryCache cache, ICacheBackplane cacheBackplane) : base(cache)
        {
            _cacheBackplane = cacheBackplane;
        }

        public override void Set(string key, object data, int? cacheTime = null)
        {
            if (data != null)
            {
                base.Set(key, data, cacheTime);
                //_cacheBackplane.NotifyChangeAsync(key, CacheItemChangedEventAction.Add);
            }
        }

        public override void Remove(string key)
        {
            Task.Run(() => _cacheBackplane.NotifyRemoveAsync(key));
        }

        public override Task RemoveAsync(string key)
        {
            return _cacheBackplane.NotifyRemoveAsync(key);
        }
    }
}
