using System.Collections.Generic;
using VirtoCommerce.CatalogModule.Web.Model;
using VirtoCommerce.Domain.Catalog.Model.Search;
using VirtoCommerce.Domain.Search;

namespace VirtoCommerce.CatalogModule.Data.Search
{
    public interface IAggregationConverter
    {
        IList<AggregationRequest> GetAggregationRequests(ProductSearchCriteria criteria, FiltersContainer allFilters);
        Aggregation[] ConvertAggregations(IList<AggregationResponse> aggregationResponses, ProductSearchCriteria criteria);
    }
}
