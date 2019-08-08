using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.CatalogModule.Core.Services;
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
    public class PriceExportPagedDataSource : BaseExportPagedDataSource
    {
        private readonly IPricingSearchService _searchService;
        private readonly IPricingService _pricingService;
        private readonly IItemService _itemService;
        private readonly IAuthorizationService _authorizationService;
        private readonly IUserClaimsResolver _userClaimsResolver;

        public PriceExportPagedDataSource(IPricingSearchService searchService,
            IPricingService pricingService,
            IItemService itemService,
            IAuthorizationService authorizationService,
            IUserClaimsResolver userClaimsResolver)
        {
            _searchService = searchService;
            _pricingService = pricingService;
            _itemService = itemService;
            _authorizationService = authorizationService;
            _userClaimsResolver = userClaimsResolver;
        }

        protected override FetchResult FetchData(SearchCriteriaBase searchCriteria)
        {
            Price[] result;
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
                result = _pricingService.GetPricesByIdAsync(Enumerable.ToArray(searchCriteria.ObjectIds)).GetAwaiter().GetResult();
                totalCount = result.Length;
            }
            else
            {
                var priceSearchResult = _searchService.SearchPricesAsync((PricesSearchCriteria)searchCriteria).GetAwaiter().GetResult();
                result = priceSearchResult.Results.ToArray();
                totalCount = priceSearchResult.TotalCount;
            }

            return new FetchResult(result, totalCount);
        }

        protected override IEnumerable<ViewableEntity> ToViewableEntities(IEnumerable<ICloneable> objects)
        {
            var models = objects.Cast<Price>();
            var viewableMap = models.ToDictionary(x => x, x => ToViewableEntity(x) as PriceViewableEntity);

            FillViewableEntitiesReferenceFields(viewableMap);

            var modelIds = models.Select(x => x.Id).ToList();
            var result = viewableMap.Values.OrderBy(x => modelIds.IndexOf(x.Id));

            return result;
        }

        protected virtual void FillViewableEntitiesReferenceFields(Dictionary<Price, PriceViewableEntity> viewableMap)
        {
            var models = viewableMap.Keys;

            var productIds = models.Select(x => x.ProductId).Distinct().ToArray();
            var pricelistIds = models.Select(x => x.PricelistId).Distinct().ToArray();
            var products = _itemService.GetByIdsAsync(productIds, ItemResponseGroup.ItemInfo.ToString()).GetAwaiter().GetResult();
            var pricelists = _pricingService.GetPricelistsByIdAsync(pricelistIds).GetAwaiter().GetResult();


            foreach (var kvp in viewableMap)
            {
                var model = kvp.Key;
                var viewableEntity = kvp.Value;
                var product = products.FirstOrDefault(x => x.Id == model.ProductId);
                var pricelist = pricelists.FirstOrDefault(x => x.Id == model.PricelistId);

                viewableEntity.Code = product?.Code;
                viewableEntity.ImageUrl = product?.ImgSrc;
                viewableEntity.Name = product?.Name;
                viewableEntity.ProductName = product?.Name;
                viewableEntity.Parent = pricelist?.Name;
                viewableEntity.PricelistName = pricelist?.Name;
            }
        }

        protected override ViewableEntity ToViewableEntity(object obj)
        {
            if (!(obj is Price model))
            {
                throw new System.InvalidCastException(nameof(Price));
            }

            var result = AbstractTypeFactory<PriceViewableEntity>.TryCreateInstance();

            result.FromEntity(model);

            result.Currency = model.Currency;
            result.EndDate = model.EndDate;
            result.List = model.List;
            result.MinQuantity = model.MinQuantity;
            result.OuterId = model.OuterId;
            result.PricelistId = model.PricelistId;
            result.ProductId = model.ProductId;
            result.Sale = model.Sale;
            result.StartDate = model.StartDate;

            return result;
        }
    }
}
