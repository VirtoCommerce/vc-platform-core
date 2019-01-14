namespace VirtoCommerce.Platform.Core.Caching
{
    public interface ICacheBackplane
    {
        void NotifyChange(string key, CacheItemChangedEventAction action);
        void NotifyChange(string key, string region, CacheItemChangedEventAction action);
        void NotifyClear();
        void NotifyClearRegion(string region);
        void NotifyRemove(string key);
        void NotifyRemove(string key, string region);
    }
}
