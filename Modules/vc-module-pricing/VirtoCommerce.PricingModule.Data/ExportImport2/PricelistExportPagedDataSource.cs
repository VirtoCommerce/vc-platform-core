using System;
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
        private readonly IPricingSearchService _searchService;
        private readonly IPricingService _pricingService;

        private ExportDataQuery _dataQuery;
        private int _totalCount = -1;

        public int CurrentPageNumber { get; protected set; }
        public int PageSize { get; set; } = 50;

        public ExportDataQuery DataQuery
        {
            set
            {
                _dataQuery = value;
                CurrentPageNumber = 0;
                _totalCount = -1;
            }
        }

        public PricelistExportPagedDataSource(IPricingSearchService searchService, IPricingService pricingService)
        {
            _searchService = searchService;
            _pricingService = pricingService;
        }

        public IEnumerable<IExportable> FetchNextPage()
        {
            var searchCriteria = BuildSearchCriteria(_dataQuery);
            var result = FetchData(searchCriteria);

            _totalCount = result.TotalCount;
            CurrentPageNumber++;

            return result.Results;
        }

        public int GetTotalCount()
        {
            if (_totalCount < 0)
            {
                var searchCriteria = BuildSearchCriteria(_dataQuery);

                searchCriteria.Skip = 0;
                searchCriteria.Take = 0;

                _totalCount = FetchData(searchCriteria).TotalCount;
            }

            return _totalCount;
        }

        protected virtual PricelistSearchCriteria BuildSearchCriteria(ExportDataQuery exportDataQuery)
        {
            var dataQuery = exportDataQuery as PricelistExportDataQuery ?? throw new InvalidCastException($"Cannot cast {nameof(exportDataQuery)} to {nameof(PricelistExportDataQuery)}");
            var result = AbstractTypeFactory<PricelistSearchCriteria>.TryCreateInstance();

            result.ObjectIds = dataQuery.ObjectIds;
            result.Keyword = dataQuery.Keyword;
            result.Sort = dataQuery.Sort;
            result.Currencies = dataQuery.Currencies;

            // It is for proper pagination - client side for viewer (dataQuery.Skip/Take) should work together with iterating through pages when getting data for export
            result.Skip = dataQuery.Skip ?? CurrentPageNumber * PageSize;
            result.Take = dataQuery.Take ?? PageSize;

            return result;
        }

        protected virtual GenericSearchResult<ExportablePricelist> FetchData(PricelistSearchCriteria searchCriteria)
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
                var pricelistSearchResult = _searchService.SearchPricelistsAsync(searchCriteria).GetAwaiter().GetResult();
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

            return new GenericSearchResult<ExportablePricelist>()
            {
                Results = result.Select(x => AbstractTypeFactory<ExportablePricelist>.TryCreateInstance().FromModel(x)).ToList(),
                TotalCount = totalCount,
            };
        }
    }
}
