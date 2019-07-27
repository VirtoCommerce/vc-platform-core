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
    public class CatalogExportPagedDataSource : BaseExportPagedDataSource
    {
        private readonly ICatalogSearchService _searchService;
        private readonly ICatalogService _catalogService;
        private ViewableEntityConverter<Catalog> _viewableEntityConverter;

        public CatalogExportPagedDataSource(ICatalogSearchService searchService, ICatalogService catalogService, IAuthorizationPolicyProvider authorizationPolicyProvider, IAuthorizationService authorizationService, IUserClaimsPrincipalFactory<ApplicationUser> userClaimsPrincipalFactory, UserManager<ApplicationUser> userManager)
            : base(authorizationPolicyProvider, authorizationService, userClaimsPrincipalFactory, userManager)
        {
            _searchService = searchService;
            _catalogService = catalogService;
        }

        protected override FetchResult FetchData(SearchCriteriaBase searchCriteria)
        {
            Catalog[] result;
            int totalCount;

            if (searchCriteria.ObjectIds.Any(x => !string.IsNullOrWhiteSpace(x)))
            {
                result = _catalogService.GetByIdsAsync(Enumerable.ToArray(searchCriteria.ObjectIds)).Result;
                totalCount = result.Length;
            }
            else
            {
                var catalogSearchResult = _searchService.SearchCatalogsAsync((CatalogSearchCriteria)searchCriteria).Result;
                result = catalogSearchResult.Results.ToArray();
                totalCount = catalogSearchResult.TotalCount;
            }

            return new FetchResult(result, totalCount);
        }

        protected override ViewableEntity ToViewableEntity(object obj)
        {
            if (!(obj is Catalog catalog))
            {
                throw new System.InvalidCastException(nameof(Catalog));
            }

            EnsureViewableConverterCreated();

            return _viewableEntityConverter.ToViewableEntity(catalog);
        }

        protected virtual void EnsureViewableConverterCreated()
        {
            if (_viewableEntityConverter == null)
            {
                _viewableEntityConverter = new ViewableEntityConverter<Catalog>(x => x.Name, x => x.OuterId, x => null, x => null);
            }
        }
    }
}
