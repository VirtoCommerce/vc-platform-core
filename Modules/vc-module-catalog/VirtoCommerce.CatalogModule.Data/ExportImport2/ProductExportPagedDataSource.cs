using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
            if (searchCriteria.ObjectIds.Any(x => !string.IsNullOrWhiteSpace(x)))
            {
                result = _itemService.GetByIdsAsync(searchCriteria.ObjectIds.ToArray(), ItemResponseGroup.Full.ToString()).GetAwaiter().GetResult();
                totalCount = result.Length;
            }
            else
            {
                var searchRsult = _productSearchService.SearchProductsAsync((ProductSearchCriteria)searchCriteria).GetAwaiter().GetResult();
                result = searchRsult.Results.ToArray();
                totalCount = searchRsult.TotalCount;
            }
            return new FetchResult(result, totalCount);
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
                _viewableEntityConverter = new ViewableEntityConverter<CatalogProduct>(x => x.Name, x => x.OuterId, x => null);
            }
        }
    }
}
