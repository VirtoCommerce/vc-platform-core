using System.Threading.Tasks;
using VirtoCommerce.CatalogModule.Core.Model;

namespace VirtoCommerce.CatalogModule.Core.Services
{
    /// <summary>
    /// The abstraction represent the CRUD operations for property dictionary items
    /// </summary>
    public interface IPropertyDictionaryItemService
    {
        Task<PropertyDictionaryItem[]> GetByIdsAsync(string[] ids);
        Task SaveChangesAsync(PropertyDictionaryItem[] dictItems);
        Task DeleteAsync(string[] ids);
    }
}
