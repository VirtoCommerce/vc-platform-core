using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using VirtoCommerce.ExportModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.Platform.Security.Authorization;
using VirtoCommerce.PricingModule.Core;
using VirtoCommerce.PricingModule.Core.Model;
using VirtoCommerce.PricingModule.Core.Model.Search;
using VirtoCommerce.PricingModule.Core.Services;

namespace VirtoCommerce.PricingModule.Data.ExportImport
{
    public class PricelistExportPagedDataSource : BaseExportPagedDataSource
    {
        private readonly IPricingSearchService _searchService;
        private readonly IPricingService _pricingService;
        private readonly IAuthorizationService _authorizationService;
        private readonly IUserClaimsResolver _userClaimsResolver;

        public PricelistExportPagedDataSource(IPricingSearchService searchService, IPricingService pricingService, IAuthorizationService authorizationService, IUserClaimsResolver userClaimsResolver)

        {
            _searchService = searchService;
            _pricingService = pricingService;
            _authorizationService = authorizationService;
            _userClaimsResolver = userClaimsResolver;
        }

        protected override FetchResult FetchData(SearchCriteriaBase searchCriteria)
        {
            Pricelist[] result;
            int totalCount;

            var claimsPrincipal = _userClaimsResolver.GetUserClaims(DataQuery.UserName).GetAwaiter().GetResult();
            var authorizationResult = _authorizationService.AuthorizeAsync(claimsPrincipal, null, new[]
            {
                new PermissionAuthorizationRequirement(ModuleConstants.Security.Permissions.Export),
                new PermissionAuthorizationRequirement(ModuleConstants.Security.Permissions.Read)
            }).GetAwaiter().GetResult();

            if (!authorizationResult.Succeeded)
            {
                throw new UnauthorizedAccessException();
            }


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

            return new FetchResult(result, totalCount);
        }

        protected override ViewableEntity ToViewableEntity(object obj)
        {
            if (!(obj is Pricelist model))
            {
                throw new System.InvalidCastException(nameof(Pricelist));
            }

            var result = AbstractTypeFactory<PricelistViewableEntity>.TryCreateInstance();

            result.FromEntity(model);

            result.Code = null;
            result.ImageUrl = null;
            result.Name = model.Name;
            result.Parent = null;

            result.Currency = model.Currency;
            result.Description = model.Description;
            result.OuterId = model.OuterId;

            return result;
        }
    }
}
