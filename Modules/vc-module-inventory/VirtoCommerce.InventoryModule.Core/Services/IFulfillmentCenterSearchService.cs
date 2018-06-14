using System.Threading.Tasks;
using VirtoCommerce.InventoryModule.Core.Model;
using VirtoCommerce.InventoryModule.Core.Model.Search;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.InventoryModule.Core.Services
{
    public interface IFulfillmentCenterSearchService
    {
        Task<GenericSearchResult<FulfillmentCenter>> SearchCentersAsync(FulfillmentCenterSearchCriteria criteria);
    }
}
