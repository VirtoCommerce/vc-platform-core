using System.Threading.Tasks;
using VirtoCommerce.CatalogModule.Core.Model;

namespace VirtoCommerce.CatalogModule.Core.Services
{
    public interface IItemService
    {
        Task<CatalogProduct[]> GetByIdsAsync(string[] itemIds, ItemResponseGroup respGroup, string catalogId = null);
        Task SaveChangesAsync(CatalogProduct[] items);
        Task DeleteAsync(string[] itemIds);
    }
}
