using System;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.CatalogModule.Core.Services;
using VirtoCommerce.ExportModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.PricingModule.Core.Model;
using VirtoCommerce.PricingModule.Core.Model.Search;
using VirtoCommerce.PricingModule.Core.Services;

namespace VirtoCommerce.PricingModule.Data.ExportImport
{
    public class PriceExportPagedDataSource : IPagedDataSource
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
        private readonly IItemService _itemService;

        public ExportDataQuery DataQuery { get; set; }
        private int _totalCount = -1;
        private SearchCriteriaBase _searchCriteria;

        public PriceExportPagedDataSource(IPricingSearchService searchService,
            IPricingService pricingService,
            IItemService itemService)
        {
            _searchService = searchService;
            _pricingService = pricingService;
            _itemService = itemService;

        }

        protected FetchResult FetchData(SearchCriteriaBase searchCriteria)
        {
            Price[] result;
            int totalCount;

            if (searchCriteria.ObjectIds.Any(x => !string.IsNullOrWhiteSpace(x)))
            {
                result = _pricingService.GetPricesByIdAsync(Enumerable.ToArray(searchCriteria.ObjectIds)).GetAwaiter().GetResult();
                totalCount = result.Length;
            }
            else
            {
                var priceSearchResult = _searchService.SearchPricesAsync((PricesSearchCriteria)searchCriteria).GetAwaiter().GetResult();
                result = priceSearchResult.Results.ToArray();
                totalCount = priceSearchResult.TotalCount;
            }

            return new FetchResult(ToExportable(result), totalCount);
        }

        protected virtual IEnumerable<IExportable> ToExportable(IEnumerable<ICloneable> objects)
        {
            var models = objects.Cast<Price>();
            var viewableMap = models.ToDictionary(x => x, x => AbstractTypeFactory<ExportablePrice>.TryCreateInstance().FromModel(x));

            FillViewableEntitiesReferenceFields(viewableMap);

            var modelIds = models.Select(x => x.Id).ToList();
            var result = viewableMap.Values.OrderBy(x => modelIds.IndexOf(x.Id));

            return result;
        }

        protected virtual void FillViewableEntitiesReferenceFields(Dictionary<Price, ExportablePrice> viewableMap)
        {
            var models = viewableMap.Keys;

            var productIds = models.Select(x => x.ProductId).Distinct().ToArray();
            var pricelistIds = models.Select(x => x.PricelistId).Distinct().ToArray();
            var products = _itemService.GetByIdsAsync(productIds, ItemResponseGroup.ItemInfo.ToString()).GetAwaiter().GetResult();
            var pricelists = _pricingService.GetPricelistsByIdAsync(pricelistIds).GetAwaiter().GetResult();


            foreach (var kvp in viewableMap)
            {
                var model = kvp.Key;
                var viewableEntity = kvp.Value;
                var product = products.FirstOrDefault(x => x.Id == model.ProductId);
                var pricelist = pricelists.FirstOrDefault(x => x.Id == model.PricelistId);

                viewableEntity.Code = product?.Code;
                viewableEntity.ImageUrl = product?.ImgSrc;
                viewableEntity.Name = product?.Name;
                viewableEntity.ProductName = product?.Name;
                viewableEntity.Parent = pricelist?.Name;
                viewableEntity.PricelistName = pricelist?.Name;
            }
        }

        public int GetTotalCount()
        {
            if (_totalCount < 0)
            {
                var searchCriteria = MakeSearchCriteria(DataQuery as PriceExportDataQuery);

                searchCriteria.Skip = 0;
                searchCriteria.Take = 0;

                var result = FetchData(searchCriteria);
                _totalCount = result.TotalCount;
            }
            return _totalCount;
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
                _searchCriteria = MakeSearchCriteria(DataQuery as PriceExportDataQuery);
            }
        }

        private PricesSearchCriteria MakeSearchCriteria(PriceExportDataQuery dataQuery)
        {
            var result = AbstractTypeFactory<PricesSearchCriteria>.TryCreateInstance();
            result.ObjectIds = dataQuery.ObjectIds;
            result.Keyword = dataQuery.Keyword;
            result.Sort = dataQuery.Sort;
            result.Skip = dataQuery.Skip ?? result.Skip;
            result.Take = dataQuery.Take ?? result.Take;
            result.PriceListIds = dataQuery.PriceListIds;
            result.ProductIds = dataQuery.ProductIds;
            result.ModifiedSince = dataQuery.ModifiedSince;
            return result;
        }
    }
}
