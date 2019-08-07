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
using VirtoCommerce.PricingModule.Data.Authorization;

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
        private readonly IAuthorizationService _authorizationService;
        private readonly IUserClaimsPrincipalFactory<ApplicationUser> _userClaimsPrincipalFactory;
        private readonly UserManager<ApplicationUser> _userManager;

        public PricelistAssignmentExportPagedDataSource(
            IPricingSearchService searchService,
            IPricingService pricingService,
            ICatalogService catalogService,
            IAuthorizationService authorizationService,
            IUserClaimsPrincipalFactory<ApplicationUser> userClaimsPrincipalFactory,
            UserManager<ApplicationUser> userManager)
            
        {
            _searchService = searchService;
            _pricingService = pricingService;
            _catalogService = catalogService;
            _authorizationService = authorizationService;
            _userClaimsPrincipalFactory = userClaimsPrincipalFactory;
            _userManager = userManager;
        }

        protected override FetchResult FetchData(SearchCriteriaBase searchCriteria)
        {
            PricelistAssignment[] result;
            int totalCount;

            var user = _userManager.FindByNameAsync(DataQuery.UserName).GetAwaiter().GetResult();
            var claimsPrincipal = _userClaimsPrincipalFactory.CreateAsync(user).GetAwaiter().GetResult();
            var authorizationResult = _authorizationService.AuthorizeAsync(claimsPrincipal, null, new PricingAuthorizationRequirement(ModuleConstants.Security.Permissions.Export)).GetAwaiter().GetResult();
            if (!authorizationResult.Succeeded)
            {
                throw new UnauthorizedAccessException();
            }


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
