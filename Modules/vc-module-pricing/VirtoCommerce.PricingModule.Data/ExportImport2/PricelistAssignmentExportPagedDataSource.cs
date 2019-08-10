using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.CatalogModule.Core.Services;
using VirtoCommerce.ExportModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.PricingModule.Core;
using VirtoCommerce.PricingModule.Core.Model;
using VirtoCommerce.PricingModule.Core.Model.Search;
using VirtoCommerce.PricingModule.Core.Services;

namespace VirtoCommerce.PricingModule.Data.ExportImport
{
    // These permissions required to fetch data
    [Authorize(ModuleConstants.Security.Permissions.Export)]
    [Authorize(ModuleConstants.Security.Permissions.Read)]
    public class PricelistAssignmentExportPagedDataSource : BaseExportPagedDataSource
    {
        private readonly IPricingSearchService _searchService;
        private readonly IPricingService _pricingService;
        private readonly ICatalogService _catalogService;

        public PricelistAssignmentExportPagedDataSource(
            IPricingSearchService searchService,
            IPricingService pricingService,
            ICatalogService catalogService,
            IAuthorizationPolicyProvider authorizationPolicyProvider,
            IAuthorizationService authorizationService,
            IUserClaimsPrincipalFactory<ApplicationUser> userClaimsPrincipalFactory,
            UserManager<ApplicationUser> userManager)
            : base(authorizationPolicyProvider, authorizationService, userClaimsPrincipalFactory, userManager)
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

            return new FetchResult(ToExportable(result), totalCount);
        }

        protected virtual IEnumerable<IExportViewable> ToExportable(IEnumerable<ICloneable> objects)
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
    }
}
