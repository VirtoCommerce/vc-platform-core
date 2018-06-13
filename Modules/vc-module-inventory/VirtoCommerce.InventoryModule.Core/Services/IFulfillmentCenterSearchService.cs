using VirtoCommerce.InventoryModule.Core.Model;
using VirtoCommerce.InventoryModule.Core.Model.Search;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.InventoryModule.Core.Services
{
    public interface IFulfillmentCenterSearchService
    {
        GenericSearchResult<FulfillmentCenter> SearchCenters(FulfillmentCenterSearchCriteria criteria);
    }
}
