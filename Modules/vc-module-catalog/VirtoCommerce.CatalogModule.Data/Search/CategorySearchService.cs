using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VirtoCommerce.CatalogModule.Core.Model.Search;
using VirtoCommerce.CatalogModule.Core.Search;
using VirtoCommerce.CatalogModule.Core.Services;
using VirtoCommerce.CatalogModule.Data.Model;
using VirtoCommerce.CatalogModule.Data.Repositories;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Data.Infrastructure;

namespace VirtoCommerce.CatalogModule.Data.Search
{
    public class CategorySearchService : ICategorySearchService
    {
        private readonly Func<ICatalogRepository> _catalogRepositoryFactory;
        private readonly ICategoryService _categoryService;
        public CategorySearchService(Func<ICatalogRepository> catalogRepositoryFactory, ICategoryService categoryService)
        {
            _catalogRepositoryFactory = catalogRepositoryFactory;
            _categoryService = categoryService;
        }

        public async Task<CategorySearchResult> SearchCategoriesAsync(CategorySearchCriteria criteria)
        {
            var result = AbstractTypeFactory<CategorySearchResult>.TryCreateInstance();

            using (var repository = _catalogRepositoryFactory())
            {
                //Optimize performance and CPU usage
                repository.DisableChangesTracking();

                var sortInfos = criteria.SortInfos;
                if (sortInfos.IsNullOrEmpty())
                {
                    sortInfos = new[]
                    {
                        new SortInfo { SortColumn = "Name" }
                    };
                }

                var query = GetQuery(repository, criteria, sortInfos);
              
                result.TotalCount = await query.CountAsync();
                if (criteria.Take > 0)
                {
                    var ids = await query.Skip(criteria.Skip).Take(criteria.Take).Select(x => x.Id).ToListAsync();
                    var categories = await _categoryService.GetByIdsAsync(ids.ToArray(), criteria.ResponseGroup);
                    result.Results = categories.OrderBy(x => ids.IndexOf(x.Id)).ToList();
                }
            }

            return result;
        }

        protected virtual IQueryable<CategoryEntity> GetQuery(ICatalogRepository repository, CategorySearchCriteria criteria, IEnumerable<SortInfo> sortInfos)
        {
            var query = repository.Categories;
            if (!string.IsNullOrEmpty(criteria.Keyword))
            {
                query = query.Where(x => x.Name.Contains(criteria.Keyword));
            }
            if (!string.IsNullOrEmpty(criteria.CatalogId))
            {
                query = query.Where(x => x.CatalogId == criteria.CatalogId);
            }
            if (!string.IsNullOrEmpty(criteria.CategoryId))
            {
                query = query.Where(x => x.ParentCategoryId == criteria.CategoryId);
            }

            query = query.OrderBySortInfos(sortInfos).ThenBy(x=>x.Id);
            return query;
        }
    }
}
