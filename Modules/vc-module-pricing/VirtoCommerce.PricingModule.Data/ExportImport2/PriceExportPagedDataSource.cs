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
        private readonly IPricingSearchService _searchService;
        private readonly IPricingService _pricingService;
        private readonly IItemService _itemService;

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

        public PriceExportPagedDataSource(IPricingSearchService searchService,
            IPricingService pricingService,
            IItemService itemService)
        {
            _searchService = searchService;
            _pricingService = pricingService;
            _itemService = itemService;
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

        protected virtual PricesSearchCriteria BuildSearchCriteria(ExportDataQuery exportDataQuery)
        {
            var dataQuery = exportDataQuery as PriceExportDataQuery ?? throw new InvalidCastException($"Cannot cast {nameof(exportDataQuery)} to {nameof(PriceExportDataQuery)}");

            var result = AbstractTypeFactory<PricesSearchCriteria>.TryCreateInstance();

            result.ObjectIds = dataQuery.ObjectIds;
            result.Keyword = dataQuery.Keyword;
            result.Sort = dataQuery.Sort;
            result.PriceListIds = dataQuery.PriceListIds;
            result.ProductIds = dataQuery.ProductIds;
            result.ModifiedSince = dataQuery.ModifiedSince;

            // It is for proper pagination - client side for viewer (dataQuery.Skip/Take) should work together with iterating through pages when getting data for export
            result.Skip = dataQuery.Skip ?? CurrentPageNumber * PageSize;
            result.Take = dataQuery.Take ?? PageSize;

            return result;
        }

        protected virtual GenericSearchResult<ExportablePrice> FetchData(PricesSearchCriteria searchCriteria)
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
                var priceSearchResult = _searchService.SearchPricesAsync(searchCriteria).GetAwaiter().GetResult();
                result = priceSearchResult.Results.ToArray();
                totalCount = priceSearchResult.TotalCount;
            }

            return new GenericSearchResult<ExportablePrice>()
            {
                Results = ToExportable(result),
                TotalCount = totalCount,
            };
        }

        protected virtual List<ExportablePrice> ToExportable(IEnumerable<ICloneable> objects)
        {
            var models = objects.Cast<Price>();
            var viewableMap = models.ToDictionary(x => x, x => AbstractTypeFactory<ExportablePrice>.TryCreateInstance().FromModel(x));

            FillAdditionalProperties(viewableMap);

            var modelIds = models.Select(x => x.Id).ToList();

            return viewableMap.Values.OrderBy(x => modelIds.IndexOf(x.Id)).ToList();
        }

        protected virtual void FillAdditionalProperties(Dictionary<Price, ExportablePrice> viewableMap)
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
    }
}
