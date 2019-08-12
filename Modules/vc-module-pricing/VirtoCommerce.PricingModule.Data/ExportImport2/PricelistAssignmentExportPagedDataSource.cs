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
    public class PricelistAssignmentExportPagedDataSource : IPagedDataSource
    {
        private readonly IPricingSearchService _searchService;
        private readonly IPricingService _pricingService;
        private readonly ICatalogService _catalogService;

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

        public PricelistAssignmentExportPagedDataSource(
            IPricingSearchService searchService,
            IPricingService pricingService,
            ICatalogService catalogService)
        {
            _searchService = searchService;
            _pricingService = pricingService;
            _catalogService = catalogService;
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

        protected virtual PricelistAssignmentsSearchCriteria BuildSearchCriteria(ExportDataQuery exportDataQuery)
        {
            var dataQuery = exportDataQuery as PricelistAssignmentExportDataQuery ?? throw new InvalidCastException($"Cannot cast {nameof(exportDataQuery)} to {nameof(PricelistAssignmentExportDataQuery)}");

            var result = AbstractTypeFactory<PricelistAssignmentsSearchCriteria>.TryCreateInstance();

            result.ObjectIds = dataQuery.ObjectIds;
            result.Keyword = dataQuery.Keyword;
            result.Sort = dataQuery.Sort;
            result.PriceListIds = dataQuery.PriceListIds;
            result.CatalogIds = dataQuery.CatalogIds;

            // It is for proper pagination - client side for viewer (dataQuery.Skip/Take) should work together with iterating through pages when getting data for export
            result.Skip = dataQuery.Skip ?? CurrentPageNumber * PageSize;
            result.Take = dataQuery.Take ?? PageSize;

            return result;
        }

        protected virtual GenericSearchResult<ExportablePricelistAssignment> FetchData(SearchCriteriaBase searchCriteria)
        {
            PricelistAssignment[] result;
            int totalCount;

            if (searchCriteria.ObjectIds.Any(x => !string.IsNullOrWhiteSpace(x)))
            {
                result = _pricingService.GetPricelistAssignmentsByIdAsync(Enumerable.ToArray(searchCriteria.ObjectIds)).GetAwaiter().GetResult();
                totalCount = result.Length;
            }
            else
            {
                var pricelistAssignmentSearchResult = _searchService.SearchPricelistAssignmentsAsync((PricelistAssignmentsSearchCriteria)searchCriteria).GetAwaiter().GetResult();
                result = pricelistAssignmentSearchResult.Results.ToArray();
                totalCount = pricelistAssignmentSearchResult.TotalCount;
            }

            return new GenericSearchResult<ExportablePricelistAssignment>()
            {
                Results = ToExportable(result).ToList(),
                TotalCount = totalCount,
            };
        }

        protected virtual IEnumerable<ExportablePricelistAssignment> ToExportable(IEnumerable<ICloneable> objects)
        {
            var models = objects.Cast<PricelistAssignment>();
            var viewableMap = models.ToDictionary(x => x, x => AbstractTypeFactory<ExportablePricelistAssignment>.TryCreateInstance().FromModel(x));

            FillAdditionalProperties(viewableMap);

            var modelIds = models.Select(x => x.Id).ToList();
            var result = viewableMap.Values.OrderBy(x => modelIds.IndexOf(x.Id));

            return result;
        }

        protected virtual void FillAdditionalProperties(Dictionary<PricelistAssignment, ExportablePricelistAssignment> viewableMap)
        {
            var models = viewableMap.Keys;
            var catalogIds = models.Select(x => x.CatalogId).Distinct().ToArray();
            var pricelistIds = models.Select(x => x.PricelistId).Distinct().ToArray();
            var catalogs = _catalogService.GetByIdsAsync(catalogIds, CatalogResponseGroup.Info.ToString()).GetAwaiter().GetResult();
            var pricelists = _pricingService.GetPricelistsByIdAsync(pricelistIds).GetAwaiter().GetResult();

            foreach (var kvp in viewableMap)
            {
                var model = kvp.Key;
                var viewableEntity = kvp.Value;
                var catalog = catalogs.FirstOrDefault(x => x.Id == model.CatalogId);
                var pricelist = pricelists.FirstOrDefault(x => x.Id == model.PricelistId);

                viewableEntity.CatalogName = catalog?.Name;
                viewableEntity.PricelistName = pricelist?.Name;
            }
        }
    }
}
