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
    [Authorize(ModuleConstants.Security.Permissions.Export)]
    [Authorize(ModuleConstants.Security.Permissions.Read)]
    public class CategoryExportPagedDataSource : BaseExportPagedDataSource
    {
        private readonly ICategorySearchService _searchService;
        private readonly ICategoryService _categoryService;
        private ViewableEntityConverter<Category> _viewableEntityConverter;

        public CategoryExportPagedDataSource(ICategorySearchService searchService, ICategoryService categoryService, IAuthorizationPolicyProvider authorizationPolicyProvider, IAuthorizationService authorizationService, IUserClaimsPrincipalFactory<ApplicationUser> userClaimsPrincipalFactory, UserManager<ApplicationUser> userManager)
            : base(authorizationPolicyProvider, authorizationService, userClaimsPrincipalFactory, userManager)
        {
            _searchService = searchService;
            _categoryService = categoryService;
        }
        protected override FetchResult FetchData(SearchCriteriaBase searchCriteria)
        {
            Category[] result;
            int totalCount;

            if (searchCriteria.ObjectIds.Any(x => !string.IsNullOrWhiteSpace(x)))
            {
                result = _categoryService.GetByIdsAsync(Enumerable.ToArray(searchCriteria.ObjectIds), "Full").Result;
                totalCount = result.Length;
            }
            else
            {
                var categorySearchResult = _searchService.SearchCategoriesAsync((CategorySearchCriteria)searchCriteria).Result;
                result = categorySearchResult.Results.ToArray();
                totalCount = categorySearchResult.TotalCount;
            }

            return new FetchResult(result, totalCount);
        }

        protected override ViewableEntity ToViewableEntity(object obj)
        {
            if (!(obj is Category category))
            {
                throw new System.InvalidCastException(nameof(Category));
            }

            EnsureViewableConverterCreated();

            return _viewableEntityConverter.ToViewableEntity(category);
        }

        protected virtual void EnsureViewableConverterCreated()
        {
            if (_viewableEntityConverter == null)
            {
                _viewableEntityConverter = new ViewableEntityConverter<Category>(x => x.Path, x => x.Code, x => x.ImgSrc);
            }
        }
    }
}
