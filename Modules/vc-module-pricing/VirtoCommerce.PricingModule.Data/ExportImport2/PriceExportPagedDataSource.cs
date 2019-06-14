using VirtoCommerce.ExportModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.PricingModule.Core.Model.Search;
using VirtoCommerce.PricingModule.Core.Services;

namespace VirtoCommerce.PricingModule.Data.ExportImport
{
    public class PriceExportPagedDataSource : BaseExportPagedDataSource
    {
        readonly IPricingSearchService _searchService;

        public PriceExportPagedDataSource(IPricingSearchService searchService)
        {
            _searchService = searchService;
        }

        protected override FetchResult FetchUsingService(SearchCriteriaBase searchCriteria)
        {
            var priceSearchResult = _searchService.SearchPricesAsync((PricesSearchCriteria)searchCriteria).Result;
            return new FetchResult(priceSearchResult.Results, priceSearchResult.TotalCount);
        }
    }
}
