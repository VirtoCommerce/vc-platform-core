using VirtoCommerce.CatalogModule.Core.Model;

namespace VirtoCommerce.CatalogModule.Core.Services
{
    /// <summary>
    /// The abstraction represent the CRUD operations for property dictionary items
    /// </summary>
    public interface IProperyDictionaryItemService
    {
        PropertyDictionaryItem[] GetByIds(string[] ids);
        void SaveChanges(PropertyDictionaryItem[] dictItems);
        void Delete(string[] ids);
    }
}
