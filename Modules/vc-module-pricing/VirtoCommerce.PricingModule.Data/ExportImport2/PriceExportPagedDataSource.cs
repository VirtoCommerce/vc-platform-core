using System.Collections;
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
    public class PriceExportPagedDataSource : BaseExportPagedDataSource
    {
        private readonly IPricingSearchService _searchService;
        private readonly IPricingService _pricingService;
        private readonly IItemService _itemService;

        public PriceExportPagedDataSource(IPricingSearchService searchService,
            IPricingService pricingService,
            IItemService itemService,
            IAuthorizationPolicyProvider authorizationPolicyProvider,
            IAuthorizationService authorizationService,
            IUserClaimsPrincipalFactory<ApplicationUser> userClaimsPrincipalFactory,
            UserManager<ApplicationUser> userManager)
            : base(authorizationPolicyProvider, authorizationService, userClaimsPrincipalFactory, userManager)
        {
            _searchService = searchService;
            _pricingService = pricingService;
            _itemService = itemService;
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

        protected override IEnumerable<ViewableEntity> ToViewableEntities(IEnumerable objects)
        {
            var prices = objects.Cast<Price>();
            var viewableMap = prices.ToDictionary(x => x, x => AbstractTypeFactory<PriceViewableEntity>.TryCreateInstance());

            FillViewableEntities(viewableMap);

            var pricesIds = prices.Select(x => x.Id).ToList();
            var result = viewableMap.Values.OrderBy(x => pricesIds.IndexOf(x.Id));

            return result;
        }

        protected virtual void FillViewableEntities(Dictionary<Price, PriceViewableEntity> viewableMap)
        {
            var prices = viewableMap.Keys;

            var productIds = prices.Select(x => x.ProductId).Distinct().ToArray();
            var pricelistIds = prices.Select(x => x.PricelistId).Distinct().ToArray();
            var products = _itemService.GetByIdsAsync(productIds, ItemResponseGroup.ItemInfo.ToString()).Result;
            var pricelists = _pricingService.GetPricelistsByIdAsync(pricelistIds).Result;


            foreach (var kvp in viewableMap)
            {
                var price = kvp.Key;
                var priceViewableEntity = kvp.Value;
                var product = products.FirstOrDefault(x => x.Id == price.ProductId);
                var pricelist = pricelists.FirstOrDefault(x => x.Id == price.PricelistId);

                priceViewableEntity.FromEntity(price);
                priceViewableEntity.Code = product?.Code;
                priceViewableEntity.Currency = price.Currency;
                priceViewableEntity.EndDate = price.EndDate;
                priceViewableEntity.ImageUrl = product?.ImgSrc;
                priceViewableEntity.List = price.List;
                priceViewableEntity.MinQuantity = price.MinQuantity;
                priceViewableEntity.Name = product?.Name;
                priceViewableEntity.OuterId = price.OuterId;
                priceViewableEntity.Parent = pricelist?.Name;
                priceViewableEntity.Pricelist = pricelist?.Name;
                priceViewableEntity.Product = product?.Name;
                priceViewableEntity.Sale = price.Sale;
                priceViewableEntity.StartDate = price.StartDate;
            }
        }
    }
}
