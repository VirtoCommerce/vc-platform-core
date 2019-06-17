using System.Threading.Tasks;
using VirtoCommerce.PricingModule.Core.Model.Search;

namespace VirtoCommerce.PricingModule.Core.Services
{
    public interface IPricingSearchService
    {
        Task<PriceSearchResult> SearchPricesAsync(PricesSearchCriteria criteria);
        Task<PricelistSearchResult> SearchPricelistsAsync(PricelistSearchCriteria criteria);
        Task<PricelistAssignmentSearchResult> SearchPricelistAssignmentsAsync(PricelistAssignmentsSearchCriteria criteria);
    }
}
