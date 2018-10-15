using System.Threading.Tasks;
using VirtoCommerce.PricingModule.Core.Model;
using VirtoCommerce.PricingModule.Core.Model.Search;

namespace VirtoCommerce.PricingModule.Core.Services
{
    public interface IPricingSearchService
    {
        Task<PricingSearchResult<Price>> SearchPricesAsync(PricesSearchCriteria criteria);
        Task<PricingSearchResult<Pricelist>> SearchPricelistsAsync(PricelistSearchCriteria criteria);
        Task<PricingSearchResult<PricelistAssignment>> SearchPricelistAssignmentsAsync(PricelistAssignmentsSearchCriteria criteria);
    }
}
