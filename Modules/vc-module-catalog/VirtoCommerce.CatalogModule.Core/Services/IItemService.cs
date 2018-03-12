using VirtoCommerce.CatalogModule.Core.Model;
namespace VirtoCommerce.CatalogModule.Core.Services
{
    public interface IItemService
    {   
        CatalogProduct[] GetByIds(string[] itemIds, string respGroup = null, string catalogId = null);
        void SaveChanges(CatalogProduct[] products);       
        void Delete(string[] itemIds);
    }
}
