using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.CatalogModule.Core.Model.ListEntry;
using VirtoCommerce.CatalogModule.Core.Model.Search;
using VirtoCommerce.CatalogModule.Core.Search;
using VirtoCommerce.CatalogModule.Core.Services;
using VirtoCommerce.CatalogModule.Data.Model;
using VirtoCommerce.CatalogModule.Data.Repositories;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Data.Infrastructure;

namespace VirtoCommerce.CatalogModule.Data.Search
{
    public class ListEntrySearchService : IListEntrySearchService
    {
        private readonly Func<ICatalogRepository> _catalogRepositoryFactory;
        private readonly IItemService _itemService;
        private readonly ICategoryService _categoryService;
        private readonly Dictionary<string, string> _productSortingAliases = new Dictionary<string, string>();
        private readonly Dictionary<string, string> _categorySortingAliases = new Dictionary<string, string>();
        public ListEntrySearchService(Func<ICatalogRepository> catalogRepositoryFactory, IItemService itemService, ICategoryService categoryService)
        {
            _catalogRepositoryFactory = catalogRepositoryFactory;
            _itemService = itemService;
            _categoryService = categoryService;

            _productSortingAliases["sku"] = nameof(CatalogProduct.Code);
            _categorySortingAliases["sku"] = nameof(Category.Code);
        }

        public async Task<ListEntrySearchResult> SearchAsync(CatalogListEntrySearchCriteria criteria)
        {
            if (criteria == null)
            {
                throw new ArgumentNullException(nameof(criteria));
            }

            criteria.Normalize();
            var result = AbstractTypeFactory<ListEntrySearchResult>.TryCreateInstance();

            //Need search in children categories if user specify keyword
            if (!string.IsNullOrEmpty(criteria.Keyword))
            {
                criteria.SearchInChildren = true;
                criteria.SearchInVariations = true;
            }

            var categorySkip = 0;
            var categoryTake = 0;
            //Because products and categories represent in search result as two separated collections for handle paging request 
            //we should join two resulting collection artificially
            //search categories
            if (criteria.ObjectTypes.IsNullOrEmpty() || criteria.ObjectTypes.Contains(nameof(Category)))
            {
                var categoriesSearchResult = await SearchCategoriesAsync(criteria);
                var categoriesTotalCount = categoriesSearchResult.TotalCount;

                categorySkip = Math.Min(categoriesTotalCount, criteria.Skip);
                categoryTake = Math.Min(criteria.Take, Math.Max(0, categoriesTotalCount - criteria.Skip));
                var categoryListEntries = categoriesSearchResult.Results.Select(x => AbstractTypeFactory<CategoryListEntry>.TryCreateInstance().FromModel(x)).ToList();

                result.TotalCount = categoriesTotalCount;
                result.ListEntries.AddRange(categoryListEntries);
            }
            if (criteria.ObjectTypes.IsNullOrEmpty() || criteria.ObjectTypes.Contains(nameof(CatalogProduct)))
            {
                criteria.Skip -= categorySkip;
                criteria.Take -= categoryTake;
                var productsSearchResult = await SearchItemsAsync(criteria);

                var productListEntries = productsSearchResult.Results.Select(x => AbstractTypeFactory<ProductListEntry>.TryCreateInstance().FromModel(x)).ToList();

                result.TotalCount += productsSearchResult.TotalCount;
                result.ListEntries.AddRange(productListEntries);
            }

            return result;
        }

        protected virtual async Task<GenericSearchResult<Category>> SearchCategoriesAsync(CatalogListEntrySearchCriteria criteria)
        {
            var result = new GenericSearchResult<Category>();
            using (var repository = _catalogRepositoryFactory())
            {
                //Optimize performance and CPU usage
                repository.DisableChangesTracking();

                var query = repository.Categories.Where(x => criteria.WithHidden || x.IsActive);

                //Get list of search in categories
                var searchCategoryIds = criteria.CategoryIds;

                if (!searchCategoryIds.IsNullOrEmpty())
                {
                    if (criteria.SearchInChildren)
                    {
                        searchCategoryIds = searchCategoryIds.Concat(await repository.GetAllChildrenCategoriesIdsAsync(searchCategoryIds)).ToArray();
                        //linked categories
                        var allLinkedCategories = repository.CategoryLinks.Where(x => searchCategoryIds.Contains(x.TargetCategoryId)).Select(x => x.SourceCategoryId).ToArray();
                        searchCategoryIds = searchCategoryIds.Concat(allLinkedCategories).Distinct().ToArray();
                    }

                    if (criteria.HideDirectLinkedCategories)
                    {
                        query = query.Where(x => searchCategoryIds.Contains(x.ParentCategoryId) || x.OutgoingLinks.Any(y => searchCategoryIds.Contains(y.TargetCategory.ParentCategoryId)));
                    }
                    else
                    {
                        query = query.Where(x => searchCategoryIds.Contains(x.ParentCategoryId) || x.OutgoingLinks.Any(y => searchCategoryIds.Contains(y.TargetCategoryId)));
                    }
                }
                else if (!criteria.CatalogIds.IsNullOrEmpty())
                {
                    if (criteria.SearchInChildren)
                    {
                        //need search in all catalog linked and children categories 
                        //First need load all categories belong to searched catalogs
                        searchCategoryIds = repository.Categories.Where(x => criteria.CatalogIds.Contains(x.CatalogId)).Select(x => x.Id).ToArray();
                        //Then load all physical categories linked to catalog
                        var allCatalogLinkedCategories = repository.CategoryLinks.Where(x => criteria.CatalogIds.Contains(x.TargetCatalogId)).Select(x => x.SourceCategoryId).ToArray();
                        searchCategoryIds = searchCategoryIds.Concat(allCatalogLinkedCategories).Distinct().ToArray();
                        //Then expand all categories, get all children's
                        searchCategoryIds = searchCategoryIds.Concat(await repository.GetAllChildrenCategoriesIdsAsync(searchCategoryIds)).ToArray();
                        if (!searchCategoryIds.IsNullOrEmpty())
                        {
                            //find all categories belong searched catalogs and all categories direct or implicitly linked to catalogs
                            query = query.Where(x => searchCategoryIds.Contains(x.Id));
                        }
                    }
                    else
                    {
                        query = query.Where(x => (criteria.CatalogIds.Contains(x.CatalogId) && x.ParentCategoryId == null) || (x.OutgoingLinks.Any(y => y.TargetCategoryId == null && criteria.CatalogIds.Contains(y.TargetCatalogId))));
                    }
                }

                if (!string.IsNullOrEmpty(criteria.Keyword))
                {
                    query = query.Where(x => x.Name.Contains(criteria.Keyword) || x.Code.Contains(criteria.Keyword));
                }
                else if (!string.IsNullOrEmpty(criteria.Code))
                {
                    query = query.Where(x => x.Code == criteria.Code);
                }
                //Extension point 
                query = BuildQuery(query, criteria);
                var sortInfos = BuildSortExpression(criteria);
                //Try to replace sorting columns names
                TryTransformSortingInfoColumnNames(_categorySortingAliases, sortInfos);

                result.TotalCount = await query.CountAsync();
                if (criteria.Take > 0)
                {
                    query = query.OrderBySortInfos(sortInfos).ThenBy(x => x.Id);

                    var categoryIds = query.Select(x => x.Id).ToList();
                    var essentialResponseGroup = CategoryResponseGroup.Info | CategoryResponseGroup.WithImages | CategoryResponseGroup.WithSeo | CategoryResponseGroup.WithLinks | CategoryResponseGroup.WithParents | CategoryResponseGroup.WithProperties | CategoryResponseGroup.WithOutlines;
                    criteria.ResponseGroup = string.Concat(criteria.ResponseGroup, ",", essentialResponseGroup.ToString());
                    result.Results = (await _categoryService.GetByIdsAsync(categoryIds.ToArray(), criteria.ResponseGroup, criteria.CatalogId)).OrderBy(x => categoryIds.IndexOf(x.Id)).ToList();
                }
            }

            return result;
        }

        protected virtual async Task<GenericSearchResult<CatalogProduct>> SearchItemsAsync(CatalogListEntrySearchCriteria criteria)
        {
            var result = new GenericSearchResult<CatalogProduct>();

            using (var repository = _catalogRepositoryFactory())
            {
                //Optimize performance and CPU usage
                repository.DisableChangesTracking();

                //list of search categories
                var searchCategoryIds = criteria.CategoryIds;
                if (criteria.SearchInChildren)
                {
                    if (!searchCategoryIds.IsNullOrEmpty())
                    {
                        searchCategoryIds = searchCategoryIds.Concat(await repository.GetAllChildrenCategoriesIdsAsync(searchCategoryIds)).ToArray();
                        //linked categories
                        var allLinkedCategories = repository.CategoryLinks.Where(x => searchCategoryIds.Contains(x.TargetCategoryId)).Select(x => x.SourceCategoryId).ToArray();
                        searchCategoryIds = searchCategoryIds.Concat(allLinkedCategories).Distinct().ToArray();
                    }
                    else if (!criteria.CatalogIds.IsNullOrEmpty())
                    {
                        //If category not specified need search in all linked and children categories
                        searchCategoryIds = repository.Categories.Where(x => criteria.CatalogIds.Contains(x.CatalogId)).Select(x => x.Id).ToArray();
                        var allCatalogLinkedCategories = repository.CategoryLinks.Where(x => criteria.CatalogIds.Contains(x.TargetCatalogId)).Select(x => x.SourceCategoryId).ToArray();
                        searchCategoryIds = searchCategoryIds.Concat(allCatalogLinkedCategories).Distinct().ToArray();
                    }
                }

                // Build the query based on the search criteria
                var query = BuildQuery(repository.Items, criteria, searchCategoryIds);
                var sortInfos = BuildSortExpression(criteria);
                //Try to replace sorting columns names
                TryTransformSortingInfoColumnNames(_productSortingAliases, sortInfos.ToArray());

                result.TotalCount = await query.CountAsync();
                if (criteria.Take > 0)
                {
                    query = query.OrderBySortInfos(sortInfos).ThenBy(x => x.Id);

                    var itemIds = query.Skip(criteria.Skip)
                                       .Take(criteria.Take)
                                       .Select(x => x.Id)
                                       .ToList();

                    var essentialResponseGroup = ItemResponseGroup.ItemInfo | ItemResponseGroup.ItemAssets | ItemResponseGroup.Links | ItemResponseGroup.Seo | ItemResponseGroup.Outlines;
                    criteria.ResponseGroup = string.Concat(criteria.ResponseGroup, ",", essentialResponseGroup.ToString());
                    result.Results = (await _itemService.GetByIdsAsync(itemIds.ToArray(), criteria.ResponseGroup, criteria.CatalogId)).OrderBy(x => itemIds.IndexOf(x.Id)).ToList();
                }
            }

            return result;
        }

        protected virtual IList<SortInfo> BuildSortExpression(CatalogListEntrySearchCriteria criteria)
        {
            var sortInfos = criteria.SortInfos;
            if (sortInfos.IsNullOrEmpty())
            {
                sortInfos = new[]
                {
                    new SortInfo { SortColumn = nameof(ItemEntity.Priority), SortDirection = SortDirection.Descending },
                    new SortInfo { SortColumn = nameof(CategoryEntity.Name) }
                };
            }
            return sortInfos;
        }


        protected virtual IQueryable<CategoryEntity> BuildQuery(IQueryable<CategoryEntity> query, CatalogListEntrySearchCriteria criteria)
        {
            return query;
        }


        protected virtual IQueryable<ItemEntity> BuildQuery(IQueryable<ItemEntity> query, CatalogListEntrySearchCriteria criteria, string[] searchCategoryIds)
        {
            query = query.Where(x => criteria.WithHidden || x.IsActive);

            if (!criteria.SearchInVariations)
            {
                query = query.Where(x => x.ParentId == null);
            }

            if (!searchCategoryIds.IsNullOrEmpty())
            {
                query = query.Where(x => searchCategoryIds.Contains(x.CategoryId) || x.CategoryLinks.Any(link => searchCategoryIds.Contains(link.CategoryId)));
            }
            else if (!criteria.CatalogIds.IsNullOrEmpty())
            {
                query = query.Where(x => criteria.CatalogIds.Contains(x.CatalogId) && (criteria.SearchInChildren || x.CategoryId == null)
                    || x.CategoryLinks.Any(link => criteria.CatalogIds.Contains(link.CatalogId) && (criteria.SearchInChildren || link.CategoryId == null)));
            }

            if (!string.IsNullOrEmpty(criteria.Code))
            {
                query = query.Where(x => x.Code == criteria.Code);
            }
            else if (!string.IsNullOrEmpty(criteria.Keyword))
            {
                query = query.Where(x => x.Name.Contains(criteria.Keyword) || x.Code.Contains(criteria.Keyword) || x.ItemPropertyValues.Any(y => y.ShortTextValue == criteria.Keyword));
            }

            if (!criteria.VendorIds.IsNullOrEmpty())
            {
                query = query.Where(x => criteria.VendorIds.Contains(x.Vendor));
            }

            if (!criteria.ProductTypes.IsNullOrEmpty())
            {
                query = query.Where(x => criteria.ProductTypes.Contains(x.ProductType));
            }

            if (criteria.OnlyBuyable != null)
            {
                query = query.Where(x => x.IsBuyable == criteria.OnlyBuyable);
            }

            if (criteria.OnlyWithTrackingInventory != null)
            {
                query = query.Where(x => x.TrackInventory == criteria.OnlyWithTrackingInventory);
            }

            return query;
        }

        protected virtual void TryTransformSortingInfoColumnNames(IDictionary<string, string> transformationMap, IEnumerable<SortInfo> sortingInfos)
        {
            //Try to replace sorting columns names
            foreach (var sortInfo in sortingInfos)
            {
                if (transformationMap.TryGetValue(sortInfo.SortColumn.ToLowerInvariant(), out var newColumnName))
                {
                    sortInfo.SortColumn = newColumnName;
                }
            }
        }
    }
}
