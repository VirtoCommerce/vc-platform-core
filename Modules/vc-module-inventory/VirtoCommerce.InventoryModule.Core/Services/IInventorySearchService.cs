using VirtoCommerce.Domain.Inventory.Model.Search;
using VirtoCommerce.InventoryModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.InventoryModule.Core.Services
{
    public interface IInventorySearchService
    {
        GenericSearchResult<InventoryInfo> SearchInventories(InventorySearchCriteria criteria);
    }
}
