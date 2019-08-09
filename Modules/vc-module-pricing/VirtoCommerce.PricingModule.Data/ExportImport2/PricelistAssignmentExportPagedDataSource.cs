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
    public class PricelistAssignmentExportPagedDataSource : BaseExportPagedDataSource
    {
        private readonly IPricingSearchService _searchService;
        private readonly IPricingService _pricingService;
        private readonly ICatalogService _catalogService;

        public PricelistAssignmentExportPagedDataSource(
            IPricingSearchService searchService,
            IPricingService pricingService,
            ICatalogService catalogService)

        {
            _searchService = searchService;
            _pricingService = pricingService;
            _catalogService = catalogService;

        }

        protected override FetchResult FetchData(SearchCriteriaBase searchCriteria)
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

            return new FetchResult(result, totalCount);
        }

        protected override ViewableEntity ToViewableEntity(object obj)
        {
            if (!(obj is PricelistAssignment model))
            {
                throw new System.InvalidCastException(nameof(PricelistAssignment));
            }

            var result = AbstractTypeFactory<PricelistAssignmentViewableEntity>.TryCreateInstance();

            result.FromEntity(model);

            result.Code = null;
            result.ImageUrl = null;
            result.Name = model.Name;
            result.Parent = null;

            result.CatalogId = model.CatalogId;
            result.Description = model.Description;
            result.EndDate = model.EndDate;
            result.PricelistId = model.PricelistId;
            result.Priority = model.Priority;
            result.StartDate = model.StartDate;

            return result;
        }

        protected override IEnumerable<ViewableEntity> ToViewableEntities(IEnumerable<ICloneable> objects)
        {
            var models = objects.Cast<PricelistAssignment>();
            var viewableMap = models.ToDictionary(x => x, x => ToViewableEntity(x) as PricelistAssignmentViewableEntity);

            FillViewableEntitiesReferenceFields(viewableMap);

            var modelIds = models.Select(x => x.Id).ToList();
            var result = viewableMap.Values.OrderBy(x => modelIds.IndexOf(x.Id));

            return result;
        }

        protected virtual void FillViewableEntitiesReferenceFields(Dictionary<PricelistAssignment, PricelistAssignmentViewableEntity> viewableMap)
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
