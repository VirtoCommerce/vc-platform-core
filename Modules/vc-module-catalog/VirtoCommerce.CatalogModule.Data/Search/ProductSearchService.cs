using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VirtoCommerce.CatalogModule.Core.Model.Search;
using VirtoCommerce.CatalogModule.Core.Search;
using VirtoCommerce.CatalogModule.Core.Services;
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
                var query = repository.Items;
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
                    query = query.Where(x => x.CategoryId == criteria.CategoryId);
                }
                if (!criteria.Skus.IsNullOrEmpty())
                {
                    query = query.Where(x => criteria.Skus.Contains(x.Code));
                }
                if (!criteria.SearchInVariations)
                {
                    query = query.Where(x => x.ParentId == null);
                }
                var sortInfos = criteria.SortInfos;
                if (sortInfos.IsNullOrEmpty())
                {
                    sortInfos = new[] { new SortInfo { SortColumn = "Name" } };
                }
                query = query.OrderBySortInfos(sortInfos).ThenBy(x => x.Id);
                result.TotalCount = await query.CountAsync();
                if (criteria.Take > 0)
                {
                    var ids = await query.Skip(criteria.Skip).Take(criteria.Take).Select(x => x.Id).ToListAsync();
                    var products = await _itemService.GetByIdsAsync(ids.ToArray(), criteria.ResponseGroup);
                    result.Results = products.OrderBy(x => ids.IndexOf(x.Id)).ToList();
                }
            }
            return result;
        }
    }
}
