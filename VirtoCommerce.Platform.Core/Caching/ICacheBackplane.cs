using System.Threading.Tasks;

namespace VirtoCommerce.Platform.Core.Caching
{
    public interface ICacheBackplane
    {
        Task NotifyChangeAsync(string key, CacheItemChangedEventAction action);
        Task NotifyChangeAsync(string key, string region, CacheItemChangedEventAction action);
        Task NotifyClearAsync();
        Task NotifyClearRegionAsync(string region);
        Task NotifyRemoveAsync(string key);
        Task NotifyRemoveAsync(string key, string region);
    }
}
