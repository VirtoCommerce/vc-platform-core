using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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

        public PricelistAssignmentExportPagedDataSource(IPricingSearchService searchService, IPricingService pricingService, IAuthorizationPolicyProvider authorizationPolicyProvider, IAuthorizationService authorizationService, IUserClaimsPrincipalFactory<ApplicationUser> userClaimsPrincipalFactory, UserManager<ApplicationUser> userManager)
            : base(authorizationPolicyProvider, authorizationService, userClaimsPrincipalFactory, userManager)
        {
            _searchService = searchService;
            _pricingService = pricingService;
        }

        protected override FetchResult FetchData(SearchCriteriaBase searchCriteria)
        {
            PricelistAssignment[] result;
            int totalCount;

            if (searchCriteria.ObjectIds.Any(x => !string.IsNullOrWhiteSpace(x)))
            {
                result = _pricingService.GetPricelistAssignmentsByIdAsync(Enumerable.ToArray(searchCriteria.ObjectIds)).Result;
                totalCount = result.Length;
            }
            else
            {
                var pricelistAssignmentSearchResult = _searchService.SearchPricelistAssignmentsAsync((PricelistAssignmentsSearchCriteria)searchCriteria).Result;
                result = pricelistAssignmentSearchResult.Results.ToArray();
                totalCount = pricelistAssignmentSearchResult.TotalCount;
            }

            return new FetchResult(result, totalCount);
        }

        protected override IEnumerable<ViewableEntity> ToViewableEntities(IEnumerable objects)
        {
            throw new System.NotImplementedException();
        }
    }
}
