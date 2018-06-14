using System.Threading.Tasks;
using VirtoCommerce.Domain.Inventory.Model.Search;
using VirtoCommerce.InventoryModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.InventoryModule.Core.Services
{
    public interface IInventorySearchService
    {
        Task<GenericSearchResult<InventoryInfo>> SearchInventoriesAsync(InventorySearchCriteria criteria);
    }
}
