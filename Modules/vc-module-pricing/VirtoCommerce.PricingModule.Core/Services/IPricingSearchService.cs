using System.Threading.Tasks;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.PricingModule.Core.Model;
using VirtoCommerce.PricingModule.Core.Model.Search;

namespace VirtoCommerce.PricingModule.Core.Services
{
    public interface IPricingSearchService
    {
        Task<GenericSearchResult<Price>> SearchPricesAsync(PricesSearchCriteria criteria);
        Task<GenericSearchResult<Pricelist>> SearchPricelistsAsync(PricelistSearchCriteria criteria);
        Task<GenericSearchResult<PricelistAssignment>> SearchPricelistAssignmentsAsync(PricelistAssignmentsSearchCriteria criteria);
    }
}
