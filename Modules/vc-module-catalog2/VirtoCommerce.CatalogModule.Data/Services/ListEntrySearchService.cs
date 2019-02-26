using System;
using System.Linq;
using VirtoCommerce.CatalogModule.Core2.Model;
using VirtoCommerce.CatalogModule.Core2.Model.ListEntry;
using VirtoCommerce.CatalogModule.Core2.Model.Search;
using VirtoCommerce.CatalogModule.Core2.Services;
using VirtoCommerce.CatalogModule.Data2.Repositories;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Data.Infrastructure;

namespace VirtoCommerce.CatalogModule.Data2.Services
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

        public GenericSearchResult<ListEntryBase> Search(CatalogListEntrySearchCriteria criteria)
        {
            var result = new GenericSearchResult<ListEntryBase>();

            using (var repository = _catalogRepositoryFactory())
            {
                //Optimize performance and CPU usage
                repository.DisableChangesTracking();

                var query = repository.Categories.Where(cat => cat.CatalogId == criteria.CatalogId && cat.ParentCategoryId == criteria.CategoryId).Select(cat => new { cat.Id, cat.Name, cat.Code })
                                                .Union(repository.Items.Where(item => item.CatalogId == criteria.CatalogId && item.CategoryId == criteria.CategoryId).Select(item => new { item.Id, item.Name, item.Code }));

                var sortInfos = criteria.SortInfos;
                if (sortInfos.IsNullOrEmpty())
                {
                    sortInfos = new[] { new SortInfo { SortColumn = "Name" } };
                }
                query = query.OrderBySortInfos(sortInfos);
                result.TotalCount = query.Count();
                var ids = query.Skip(criteria.Skip).Take(criteria.Take).Select(x => x.Id).ToList();
                var productsListEntries = _itemService.GetByIds(ids, (ItemResponseGroup.ItemInfo | ItemResponseGroup.ItemAssets | ItemResponseGroup.Outlines).ToString()).Select(x=> AbstractTypeFactory<ProductListEntry>.TryCreateInstance().FromModel(x));
                var categoriesListEntries = _categoryService.GetByIds(ids, (CategoryResponseGroup.Info | CategoryResponseGroup.WithImages | CategoryResponseGroup.WithOutlines).ToString()).Select(x => AbstractTypeFactory<CategoryListEntry>.TryCreateInstance().FromModel(x)); ;
                result.Results = productsListEntries.Concat(categoriesListEntries).OrderBy(x => ids.IndexOf(x.Id)).ToList();
            }
            return result;
        }
    }
}
