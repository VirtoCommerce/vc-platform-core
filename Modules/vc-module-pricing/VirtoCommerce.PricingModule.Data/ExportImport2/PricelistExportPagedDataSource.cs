using System.Linq;
using VirtoCommerce.ExportModule.Core.Model;
using VirtoCommerce.ExportModule.Data.Services;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.PricingModule.Core.Model;
using VirtoCommerce.PricingModule.Core.Model.Search;
using VirtoCommerce.PricingModule.Core.Services;

namespace VirtoCommerce.PricingModule.Data.ExportImport
{
    public class PricelistExportPagedDataSource : SingleTypeExportPagedDataSource<PricelistExportDataQuery, PricelistSearchCriteria>
    {
        private readonly IPricingSearchService _searchService;
        private readonly IPricingService _pricingService;

        public PricelistExportPagedDataSource(IPricingSearchService searchService, IPricingService pricingService)
        {
            _searchService = searchService;
            _pricingService = pricingService;
        }

        protected override void FillSearchCriteria(PricelistExportDataQuery dataQuery, PricelistSearchCriteria searchCriteria)
        {
            searchCriteria.Currencies = dataQuery.Currencies;
        }

        protected override GenericSearchResult<IExportable> FetchData(PricelistSearchCriteria searchCriteria)
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

            return new GenericSearchResult<IExportable>()
            {
                Results = result.Select(x => (IExportable)AbstractTypeFactory<ExportablePricelist>.TryCreateInstance().FromModel(x)).ToList(),
                TotalCount = totalCount,
            };
        }
    }
}
