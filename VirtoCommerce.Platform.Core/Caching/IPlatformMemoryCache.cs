using Microsoft.Extensions.Caching.Memory;

namespace VirtoCommerce.Platform.Core.Caching
{
    public interface IPlatformMemoryCache : IMemoryCache
    {
        void Set(string key, object data, int? cacheTime = null);
    }
}
