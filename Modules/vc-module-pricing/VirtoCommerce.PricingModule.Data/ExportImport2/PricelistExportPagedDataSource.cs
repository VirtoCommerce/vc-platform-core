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
    public class PricelistExportPagedDataSource : BaseExportPagedDataSource
    {
        private readonly IPricingSearchService _searchService;
        private readonly IPricingService _pricingService;

        public PricelistExportPagedDataSource(IPricingSearchService searchService, IPricingService pricingService, IAuthorizationPolicyProvider authorizationPolicyProvider, IAuthorizationService authorizationService, IUserClaimsPrincipalFactory<ApplicationUser> userClaimsPrincipalFactory, UserManager<ApplicationUser> userManager)
            : base(authorizationPolicyProvider, authorizationService, userClaimsPrincipalFactory, userManager)
        {
            _searchService = searchService;
            _pricingService = pricingService;
        }

        protected override FetchResult FetchData(SearchCriteriaBase searchCriteria)
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
                var pricelistSearchResult = _searchService.SearchPricelistsAsync((PricelistSearchCriteria)searchCriteria).GetAwaiter().GetResult();
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

            return new FetchResult(result.Select(x => ExportablePricelist.FromModel(x)), totalCount);
        }
    }
}
