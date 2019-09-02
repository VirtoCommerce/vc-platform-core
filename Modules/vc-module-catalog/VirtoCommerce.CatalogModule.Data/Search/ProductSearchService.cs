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
    public class ProductSearchService : IProductSearchService
    {
        private readonly Func<ICatalogRepository> _catalogRepositoryFactory;
        private readonly IItemService _itemService;
        public ProductSearchService(Func<ICatalogRepository> catalogRepositoryFactory, IItemService itemService)
        {
            _catalogRepositoryFactory = catalogRepositoryFactory;
            _itemService = itemService;
        }

        public async Task<ProductSearchResult> SearchProductsAsync(ProductSearchCriteria criteria)
        {
            var result = AbstractTypeFactory<ProductSearchResult>.TryCreateInstance();

            using (var repository = _catalogRepositoryFactory())
            {
                //Optimize performance and CPU usage
                repository.DisableChangesTracking();

                var sortInfos = BuildSortExpression(criteria);
                var query = BuildQuery(repository, criteria);

                result.TotalCount = await query.CountAsync();
                if (criteria.Take > 0)
                {
                    var ids = await query.OrderBySortInfos(sortInfos).ThenBy(x => x.Id)
                                        .Select(x => x.Id)
                                        .Skip(criteria.Skip).Take(criteria.Take)
                                        .ToArrayAsync();

                    result.Results = (await _itemService.GetByIdsAsync(ids, criteria.ResponseGroup)).OrderBy(x => Array.IndexOf(ids, x.Id)).ToList();
                }
            }
            return result;
        }

        protected virtual IQueryable<ItemEntity> BuildQuery(ICatalogRepository repository, ProductSearchCriteria criteria)
        {
            var query = repository.Items;

            if (!string.IsNullOrEmpty(criteria.Keyword))
            {
                query = query.Where(x => x.Name.Contains(criteria.Keyword));
            }

            if (!criteria.CategoryIds.IsNullOrEmpty())
            {
                var searchCategoryIds = criteria.CategoryIds;

                if (criteria.SearchInChildren)
                {
                    searchCategoryIds = searchCategoryIds.Concat(repository.GetAllChildrenCategoriesIdsAsync(searchCategoryIds).GetAwaiter().GetResult()).ToArray();
                }

                query = query.Where(x => searchCategoryIds.Contains(x.CategoryId) || x.CategoryLinks.Any(link => searchCategoryIds.Contains(link.CategoryId)));
            }

            if (!criteria.CatalogIds.IsNullOrEmpty())
            {
                query = query.Where(x => criteria.CatalogIds.Contains(x.CatalogId));
            }

            if (!criteria.Skus.IsNullOrEmpty())
            {
                query = query.Where(x => criteria.Skus.Contains(x.Code));
            }

            if (!criteria.SearchInVariations)
            {
                query = query.Where(x => x.ParentId == null);
            }

            return query;
        }

        protected virtual IList<SortInfo> BuildSortExpression(ProductSearchCriteria criteria)
        {
            var sortInfos = criteria.SortInfos;
            if (sortInfos.IsNullOrEmpty())
            {
                sortInfos = new[]
                {
                    new SortInfo { SortColumn = nameof(ItemEntity.Name) }
                };
            }

            return sortInfos;
        }
    }
}
