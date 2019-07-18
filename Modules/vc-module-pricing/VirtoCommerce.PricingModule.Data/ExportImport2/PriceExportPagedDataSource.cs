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
    public class PriceExportPagedDataSource : BaseExportPagedDataSource
    {
        readonly IPricingSearchService _searchService;
        private readonly IPricingService _pricingService;

        public PriceExportPagedDataSource(IPricingSearchService searchService, IPricingService pricingService, IAuthorizationPolicyProvider authorizationPolicyProvider, IAuthorizationService authorizationService, IUserClaimsPrincipalFactory<ApplicationUser> userClaimsPrincipalFactory, UserManager<ApplicationUser> userManager)
            : base(authorizationPolicyProvider, authorizationService, userClaimsPrincipalFactory, userManager)
        {
            _searchService = searchService;
            _pricingService = pricingService;
        }

        protected override FetchResult FetchData(SearchCriteriaBase searchCriteria)
        {
            Price[] result;
            int totalCount;

            if (searchCriteria.ObjectIds.Any(x => !string.IsNullOrWhiteSpace(x)))
            {
                result = _pricingService.GetPricesByIdAsync(Enumerable.ToArray(searchCriteria.ObjectIds)).Result;
                totalCount = result.Length;
            }
            else
            {
                var priceSearchResult = _searchService.SearchPricesAsync((PricesSearchCriteria)searchCriteria).Result;
                result = priceSearchResult.Results.ToArray();
                totalCount = priceSearchResult.TotalCount;
            }

            return new FetchResult(result, totalCount);
        }
    }
}
