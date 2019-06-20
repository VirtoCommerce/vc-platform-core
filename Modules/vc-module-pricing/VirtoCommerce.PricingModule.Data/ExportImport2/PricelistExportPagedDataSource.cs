using VirtoCommerce.ExportModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.PricingModule.Core.Model.Search;
using VirtoCommerce.PricingModule.Core.Services;

namespace VirtoCommerce.PricingModule.Data.ExportImport
{
    public class PricelistExportPagedDataSource : BaseExportPagedDataSource
    {
        readonly IPricingSearchService _searchService;

        public PricelistExportPagedDataSource(IPricingSearchService searchService)
        {
            _searchService = searchService;
        }

        protected override FetchResult FetchData(SearchCriteriaBase searchCriteria)
        {
            var pricelistSearchResult = _searchService.SearchPricelistsAsync((PricelistSearchCriteria)searchCriteria).Result;
            return new FetchResult(pricelistSearchResult.Results, pricelistSearchResult.TotalCount);
        }
    }
}
