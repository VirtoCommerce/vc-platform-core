using System.Collections.Generic;
using System.Threading.Tasks;
using VirtoCommerce.CatalogModule.Core.Model.Search;
using VirtoCommerce.SearchModule.Core.Model;

namespace VirtoCommerce.CatalogModule.Core.Search
{
    public interface IAggregationConverter
    {
        Task<IList<AggregationRequest>> GetAggregationRequestsAsync(ProductSearchCriteria criteria, FiltersContainer allFilters);
        Task<Aggregation[]> ConvertAggregationsAsync(IList<AggregationResponse> aggregationResponses, ProductSearchCriteria criteria);
    }
}
