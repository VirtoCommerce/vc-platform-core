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
        private readonly ICatalogService _catalogService;

        public ExportDataQuery DataQuery { get; set; }
        private int _totalCount = -1;
        private SearchCriteriaBase _searchCriteria;

        public PricelistAssignmentExportPagedDataSource(
            IPricingSearchService searchService,
            IPricingService pricingService,
            ICatalogService catalogService)

        {
            _searchService = searchService;
            _pricingService = pricingService;
            _catalogService = catalogService;

        }

        protected FetchResult FetchData(SearchCriteriaBase searchCriteria)
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

            return new FetchResult(ToExportable(result), totalCount);
        }

        protected virtual IEnumerable<IExportable> ToExportable(IEnumerable<ICloneable> objects)
        {
            var models = objects.Cast<PricelistAssignment>();
            var viewableMap = models.ToDictionary(x => x, x => AbstractTypeFactory<ExportablePricelistAssignment>.TryCreateInstance().FromModel(x));

            FillViewableEntitiesReferenceFields(viewableMap);

            var modelIds = models.Select(x => x.Id).ToList();
            var result = viewableMap.Values.OrderBy(x => modelIds.IndexOf(x.Id));

            return result;
        }

        protected virtual void FillViewableEntitiesReferenceFields(Dictionary<PricelistAssignment, ExportablePricelistAssignment> viewableMap)
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

        public int GetTotalCount()
        {
            if (_totalCount < 0)
            {
                var searchCriteria = MakeSearchCriteria(DataQuery as PricelistAssignmentExportDataQuery);

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
                _searchCriteria = MakeSearchCriteria(DataQuery as PricelistAssignmentExportDataQuery);
            }
        }

        private PricelistAssignmentsSearchCriteria MakeSearchCriteria(PricelistAssignmentExportDataQuery dataQuery)
        {
            var result = AbstractTypeFactory<PricelistAssignmentsSearchCriteria>.TryCreateInstance();
            result.ObjectIds = dataQuery.ObjectIds;
            result.Keyword = dataQuery.Keyword;
            result.Sort = dataQuery.Sort;
            result.Skip = dataQuery.Skip ?? result.Skip;
            result.Take = dataQuery.Take ?? result.Take;
            result.PriceListIds = dataQuery.PriceListIds;
            result.PriceListIds = dataQuery.PriceListIds;
            result.CatalogIds = dataQuery.CatalogIds;
            return result;
        }
    }
}
