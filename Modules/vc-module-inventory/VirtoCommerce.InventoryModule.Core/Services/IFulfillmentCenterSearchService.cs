using System.Threading.Tasks;
using VirtoCommerce.InventoryModule.Core.Model.Search;

namespace VirtoCommerce.InventoryModule.Core.Services
{
    public interface IFulfillmentCenterSearchService
    {
        Task<FulfillmentCenterSearchResult> SearchCentersAsync(FulfillmentCenterSearchCriteria criteria);
    }
}
