using System;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.CatalogModule.Core.Model.ListEntry;
using VirtoCommerce.CatalogModule.Core.Services;
using VirtoCommerce.CatalogModule.Data.Repositories;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Data.Infrastructure;
using Microsoft.EntityFrameworkCore;
using VirtoCommerce.CatalogModule.Core.Model.Search;
using VirtoCommerce.CatalogModule.Core.Search;

namespace VirtoCommerce.CatalogModule.Data.Search
{
    public class ListEntrySearchService : IListEntrySearchService
    {
        private readonly Func<ICatalogRepository> _catalogRepositoryFactory;
        private readonly IItemService _itemService;
        private readonly ICategoryService _categoryService;
        public ListEntrySearchService(Func<ICatalogRepository> catalogRepositoryFactory, IItemService itemService, ICategoryService categoryService)
        {
            _catalogRepositoryFactory = catalogRepositoryFactory;
            _itemService = itemService;
            _categoryService = categoryService;
        }

        public async Task<ListEntrySearchResult> SearchAsync(CatalogListEntrySearchCriteria criteria)
        {
            var result = AbstractTypeFactory<ListEntrySearchResult>.TryCreateInstance();

            using (var repository = _catalogRepositoryFactory())
            {
                //Optimize performance and CPU usage
                repository.DisableChangesTracking();
                //TODO: Virtual categories and links, Variations, Deep recursive search

                var query = repository.Categories.Where(cat => cat.CatalogId == criteria.CatalogId && cat.ParentCategoryId == criteria.CategoryId).Select(cat => new { cat.Id, cat.Name, cat.Code })
                                                .Union(repository.Items.Where(item => item.CatalogId == criteria.CatalogId && item.CategoryId == criteria.CategoryId).Select(item => new { item.Id, item.Name, item.Code }));

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
                    var productsListEntries = (await _itemService.GetByIdsAsync(ids.ToArray(), (ItemResponseGroup.ItemInfo | ItemResponseGroup.ItemAssets | ItemResponseGroup.Outlines).ToString())).Select(x => AbstractTypeFactory<ProductListEntry>.TryCreateInstance().FromModel(x)).ToList();
                    var categoriesListEntries = (await _categoryService.GetByIdsAsync(ids.ToArray(), (CategoryResponseGroup.Info | CategoryResponseGroup.WithImages | CategoryResponseGroup.WithOutlines).ToString())).Select(x => AbstractTypeFactory<CategoryListEntry>.TryCreateInstance().FromModel(x)).ToList();
                    result.Results = productsListEntries.Concat(categoriesListEntries).OrderBy(x => ids.IndexOf(x.Id)).ToList();
                }
            }
            return result;
        }
    }
}
