using System.Threading.Tasks;
using VirtoCommerce.Domain.Inventory.Model.Search;
using VirtoCommerce.InventoryModule.Core.Model.Search;

namespace VirtoCommerce.InventoryModule.Core.Services
{
    public interface IInventorySearchService
    {
        Task<InventoryInfoSearchResult> SearchInventoriesAsync(InventorySearchCriteria criteria);
    }
}
