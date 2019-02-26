using System.Collections.Generic;
using VirtoCommerce.CatalogModule.Core.Model;
namespace VirtoCommerce.CatalogModule.Core.Services
{
    public interface IItemService
    {
        IEnumerable<CatalogProduct> GetByIds(IEnumerable<string> itemIds, string respGroup = null, string catalogId = null);
        void SaveChanges(IEnumerable<CatalogProduct> products);       
        void Delete(IEnumerable<string> itemIds);
        void LoadDependencies(IEnumerable<CatalogProduct> products);
    }
}
