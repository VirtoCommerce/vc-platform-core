using System.Threading.Tasks;
using VirtoCommerce.CatalogModule.Core.Model;

namespace VirtoCommerce.CatalogModule.Core.Services
{
    public interface IItemService
    {
        Task<CatalogProduct> GetByIdAsync(string itemId, ItemResponseGroup respGroup, string catalogId = null);
        Task<CatalogProduct[]> GetByIdsAsync(string[] itemIds, ItemResponseGroup respGroup, string catalogId = null);
        Task<CatalogProduct> CreateAsync(CatalogProduct item);
        Task CreateAsync(CatalogProduct[] items);
        Task UpdateAsync(CatalogProduct[] items);
        Task DeleteAsync(string[] itemIds);
    }
}
