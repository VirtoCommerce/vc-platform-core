using System;
using System.Threading.Tasks;

namespace VirtoCommerce.Platform.Core.Caching
{
    public interface ICacheManager
    {
        T Get<T>(string key, Func<T> factory, int? cacheTime = null);
        Task<T> GetAsync<T>(string key, Func<Task<T>> factory, int? cacheTime = null);
        void Set(string key, object data, int? cacheTime = null);
        void Remove(string key);
        Task RemoveAsync(string key);
        void RemoveByPattern(string pattern);
        Task RemoveByPatternAsync(string pattern);
        void Clear();
    }
}
