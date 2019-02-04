using System.Linq;
using System.Text.RegularExpressions;
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
            }
        }

        public override void Remove(string key)
        {
            Task.Run(() => _cacheBackplane.NotifyRemoveAsync(RemoveKey(key)));
        }

        public override Task RemoveAsync(string key)
        {
            return _cacheBackplane.NotifyRemoveAsync(RemoveKey(key));
        }

        public override void RemoveByPattern(string pattern)
        {
            var regex = new Regex(pattern, RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase);
            var matchesKeys = _allKeys.Where(p => p.Value).Select(p => p.Key).Where(key => regex.IsMatch(key)).ToList();
            var tasks = matchesKeys.Select(k => _cacheBackplane.NotifyRemoveAsync(RemoveKey(k))).ToArray();
            Task.WaitAll(tasks);
        }

        public override Task RemoveByPatternAsync(string pattern)
        {
            RemoveByPattern(pattern);
            return Task.CompletedTask;
        }
    }
}
