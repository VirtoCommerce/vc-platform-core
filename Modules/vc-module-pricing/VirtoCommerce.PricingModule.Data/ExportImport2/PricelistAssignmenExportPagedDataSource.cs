using VirtoCommerce.ExportModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.PricingModule.Core.Model.Search;
using VirtoCommerce.PricingModule.Core.Services;

namespace VirtoCommerce.PricingModule.Data.ExportImport
{
    public class PricelistAssignmenExportPagedDataSource : BaseExportPagedDataSource
    {
        readonly IPricingSearchService _searchService;

        public PricelistAssignmenExportPagedDataSource(IPricingSearchService searchService)
        {
            _searchService = searchService;
        }

        protected override FetchResult FetchUsingService(SearchCriteriaBase searchCriteria)
        {
            var pricelistAssignmentSearchResult = _searchService.SearchPricelistAssignmentsAsync((PricelistAssignmentsSearchCriteria)searchCriteria).Result;
            return new FetchResult(pricelistAssignmentSearchResult.Results, pricelistAssignmentSearchResult.TotalCount);
        }
    }
}
