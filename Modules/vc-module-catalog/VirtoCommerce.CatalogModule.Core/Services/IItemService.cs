using VirtoCommerce.CatalogModule.Core.Model;

namespace VirtoCommerce.CatalogModule.Core.Services
{
    public interface IItemService
    {
        CatalogProduct GetById(string itemId, ItemResponseGroup respGroup, string catalogId = null);
        CatalogProduct[] GetByIds(string[] itemIds, ItemResponseGroup respGroup, string catalogId = null);
        CatalogProduct Create(CatalogProduct item);
        void Create(CatalogProduct[] items);
        void Update(CatalogProduct[] items);
        void Delete(string[] itemIds);
    }
}
