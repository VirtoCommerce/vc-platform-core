using System.Threading.Tasks;

namespace VirtoCommerce.Platform.Core.DynamicProperties
{
    /// <summary>
    /// Represent runtime object extension functionality based on properties meta-information 
    /// </summary>
    public interface IDynamicPropertyService
    {
        Task<DynamicProperty[]> GetDynamicPropertiesAsync(string[] ids);
        /// <summary>
        /// Update or create dynamic properties 
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
        Task<DynamicProperty[]> SaveDynamicPropertiesAsync(DynamicProperty[] properties);
        Task DeleteDynamicPropertiesAsync(string[] propertyIds);

        Task<DynamicPropertyDictionaryItem[]> GetDynamicPropertyDictionaryItemsAsync(string[] ids);
        Task SaveDictionaryItemsAsync(DynamicPropertyDictionaryItem[] items);
        Task DeleteDictionaryItemsAsync(string[] itemIds);

        /// <summary>
        /// Deep loads and populate dynamic properties values for objects
        /// </summary>
        /// <param name="owner"></param>
        Task LoadDynamicPropertyValuesAsync(params IHasDynamicProperties[] owner);
        /// <summary>
        /// Deep save dynamic properties values for object
        /// </summary>
        /// <param name="owner"></param>
        Task SaveDynamicPropertyValuesAsync(IHasDynamicProperties owner);
        Task DeleteDynamicPropertyValuesAsync(IHasDynamicProperties owner);
    }
}
