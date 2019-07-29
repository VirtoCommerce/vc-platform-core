using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Internal;
using VirtoCommerce.CatalogModule.Core;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.CatalogModule.Core.Model.Search;
using VirtoCommerce.CatalogModule.Core.Search;
using VirtoCommerce.CatalogModule.Core.Services;
using VirtoCommerce.ExportModule.Core.Model;
using VirtoCommerce.ExportModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Security;

namespace VirtoCommerce.CatalogModule.Data.ExportImport
{
    // These permissions required to fetch catalog data
    [Authorize(ModuleConstants.Security.Permissions.Export)]
    [Authorize(ModuleConstants.Security.Permissions.Read)]

    public class ProductExportPagedDataSource : BaseExportPagedDataSource
    {
        private readonly IProductSearchService _productSearchService;
        private readonly IItemService _itemService;
        private ViewableEntityConverter<CatalogProduct> _viewableEntityConverter;

        public ProductExportPagedDataSource(IProductSearchService productSearchService, IItemService itemService, IAuthorizationPolicyProvider authorizationPolicyProvider, IAuthorizationService authorizationService, IUserClaimsPrincipalFactory<ApplicationUser> userClaimsPrincipalFactory, UserManager<ApplicationUser> userManager)
            : base(authorizationPolicyProvider, authorizationService, userClaimsPrincipalFactory, userManager)
        {
            _productSearchService = productSearchService;
            _itemService = itemService;
        }

        protected override FetchResult FetchData(SearchCriteriaBase searchCriteria)
        {
            CatalogProduct[] result;
            int totalCount;
            var responseGroup = GetResponseGroup();
            if (searchCriteria.ObjectIds.Any(x => !string.IsNullOrWhiteSpace(x)))
            {
                result = _itemService.GetByIdsAsync(searchCriteria.ObjectIds.ToArray(), responseGroup.ToString()).GetAwaiter().GetResult();
                totalCount = result.Length;
            }
            else
            {
                searchCriteria.ResponseGroup = responseGroup.ToString();
                var searchRsult = _productSearchService.SearchProductsAsync((ProductSearchCriteria)searchCriteria).GetAwaiter().GetResult();
                result = searchRsult.Results.ToArray();
                totalCount = searchRsult.TotalCount;
            }
            return new FetchResult(result, totalCount);
        }

        private ItemResponseGroup GetResponseGroup()
        {
            var result = ItemResponseGroup.ItemInfo;
            if (DataQuery.IncludedColumns.IsNullOrEmpty())
            {
                result |= ItemResponseGroup.Full;
            }
            else
            {
                if (DataQuery.IncludedColumns.Any(x => x.Group.StartsWith($"{nameof(CatalogProduct)}.{nameof(CatalogProduct.Variations)}")))
                {
                    result |= ItemResponseGroup.WithVariations;
                }

                if (DataQuery.IncludedColumns.Any(x => x.Group.StartsWith($"{nameof(CatalogProduct)}.{nameof(CatalogProduct.Inventories)}")))
                {
                    result |= ItemResponseGroup.Inventory;
                }

                if (DataQuery.IncludedColumns.Any(x => x.Group.StartsWith($"{nameof(CatalogProduct)}.{nameof(CatalogProduct.Assets)}")))
                {
                    result |= ItemResponseGroup.ItemAssets;
                }

                if (DataQuery.IncludedColumns.Any(x => x.Group.StartsWith($"{nameof(CatalogProduct)}.{nameof(CatalogProduct.Associations)}")))
                {
                    result |= ItemResponseGroup.ItemAssociations;
                }

                if (DataQuery.IncludedColumns.Any(x => x.Group.StartsWith($"{nameof(CatalogProduct)}.{nameof(CatalogProduct.Properties)}")))
                {
                    result |= ItemResponseGroup.WithProperties;
                }

                if (DataQuery.IncludedColumns.Any(x => x.Group.StartsWith($"{nameof(CatalogProduct)}.{nameof(CatalogProduct.Outlines)}")))
                {
                    result |= ItemResponseGroup.WithOutlines;
                }

                if (DataQuery.IncludedColumns.Any(x => x.Group.StartsWith($"{nameof(CatalogProduct)}.{nameof(CatalogProduct.SeoInfos)}")))
                {
                    result |= ItemResponseGroup.WithSeo;
                }

                if (DataQuery.IncludedColumns.Any(x => x.Group.StartsWith($"{nameof(CatalogProduct)}.{nameof(CatalogProduct.Links)}")))
                {
                    result |= ItemResponseGroup.WithLinks;
                }

                if (DataQuery.IncludedColumns.Any(x => x.Group.StartsWith($"{nameof(CatalogProduct)}.{nameof(CatalogProduct.Reviews)}")))
                {
                    result |= ItemResponseGroup.ItemEditorialReviews;
                }

            }
            return result;
        }

        protected override ViewableEntity ToViewableEntity(object obj)
        {
            if (!(obj is CatalogProduct catalogProduct))
            {
                throw new System.InvalidCastException(nameof(CatalogProduct));
            }

            EnsureViewableConverterCreated();

            return _viewableEntityConverter.ToViewableEntity(catalogProduct);
        }

        protected virtual void EnsureViewableConverterCreated()
        {
            if (_viewableEntityConverter == null)
            {
                _viewableEntityConverter = new ViewableEntityConverter<CatalogProduct>(x => x.Name, x => x.OuterId, x => null, x => null);
            }
        }
    }
}
