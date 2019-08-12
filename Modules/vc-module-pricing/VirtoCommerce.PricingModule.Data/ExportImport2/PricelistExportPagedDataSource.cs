using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.ExportModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.PricingModule.Core.Model;
using VirtoCommerce.PricingModule.Core.Model.Search;
using VirtoCommerce.PricingModule.Core.Services;

namespace VirtoCommerce.PricingModule.Data.ExportImport
{
    public class PricelistExportPagedDataSource : IPagedDataSource
    {
        protected class FetchResult
        {
            public IEnumerable<IExportable> Results { get; set; }
            public int TotalCount { get; set; }

            public FetchResult(IEnumerable<IExportable> results, int totalCount)
            {
                Results = results;
                TotalCount = totalCount;
            }
        }

        private readonly IPricingSearchService _searchService;
        private readonly IPricingService _pricingService;

        public PricelistExportPagedDataSource(IPricingSearchService searchService, IPricingService pricingService)

        {
            _searchService = searchService;
            _pricingService = pricingService;
        }

        public ExportDataQuery DataQuery { get; set; }
        private int _totalCount = -1;
        private SearchCriteriaBase _searchCriteria;

        protected FetchResult FetchData(SearchCriteriaBase searchCriteria)
        {
            Pricelist[] result;
            int totalCount;

            if (searchCriteria.ObjectIds.Any(x => !string.IsNullOrWhiteSpace(x)))
            {
                result = _pricingService.GetPricelistsByIdAsync(searchCriteria.ObjectIds.ToArray()).GetAwaiter().GetResult();
                totalCount = result.Length;
            }
            else
            {
                var pricelistSearchResult = _searchService.SearchPricelistsAsync((PricelistSearchCriteria)searchCriteria).GetAwaiter().GetResult();
                result = pricelistSearchResult.Results.ToArray();
                totalCount = pricelistSearchResult.TotalCount;
            }

            if (!result.IsNullOrEmpty())
            {
                var pricelistIds = result.Select(x => x.Id).ToArray();
                var prices = _searchService.SearchPricesAsync(new PricesSearchCriteria() { PriceListIds = pricelistIds, Take = int.MaxValue }).GetAwaiter().GetResult();
                foreach (var pricelist in result)
                {
                    pricelist.Prices = prices.Results.Where(x => x.PricelistId == pricelist.Id).ToArray();
                }
            }

            return new FetchResult(result.Select(x => AbstractTypeFactory<ExportablePricelist>.TryCreateInstance().FromModel(x)), totalCount);
        }

        public IEnumerable<IExportable> FetchNextPage()
        {
            EnsureSearchCriteriaInitialized();
            var result = FetchData(_searchCriteria);
            _totalCount = result.TotalCount;
            _searchCriteria.Skip += _searchCriteria.Take;
            return result.Results;
        }

        private void EnsureSearchCriteriaInitialized()
        {
            if (_searchCriteria == null)
            {
                _searchCriteria = MakeSearchCriteria(DataQuery);
            }
        }

        public int GetTotalCount()
        {
            if (_totalCount < 0)
            {
                var searchCriteria = MakeSearchCriteria(DataQuery);

                searchCriteria.Skip = 0;
                searchCriteria.Take = 0;

                var result = FetchData(searchCriteria);
                _totalCount = result.TotalCount;
            }
            return _totalCount;
        }

        private PricelistSearchCriteria MakeSearchCriteria(ExportDataQuery dataQuery)
        {
            var result = AbstractTypeFactory<PricelistSearchCriteria>.TryCreateInstance();
            result.ObjectIds = dataQuery.ObjectIds;
            result.Keyword = dataQuery.Keyword;
            result.Sort = dataQuery.Sort;
            result.Skip = dataQuery.Skip ?? result.Skip;
            result.Take = dataQuery.Take ?? result.Take;
            if (DataQuery is PricelistExportDataQuery)
            {
                result.Currencies = ((PricelistExportDataQuery)dataQuery).Currencies;
            }
            return result;
        }
    }
}
