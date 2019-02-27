using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.CatalogModule.Data.Extensions;
using VirtoCommerce.CatalogModule.Data.Model;
using VirtoCommerce.CatalogModule.Data.Repositories;
using VirtoCommerce.Domain.Catalog.Model;
using VirtoCommerce.Domain.Catalog.Services;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Data.Infrastructure;

namespace VirtoCommerce.CatalogModule.Data.Services
{
    public class CatalogSearchServiceImpl : ICatalogSearchService
    {
        private readonly Func<ICatalogRepository> _catalogRepositoryFactory;
        private readonly IItemService _itemService;
        private readonly ICatalogService _catalogService;
        private readonly ICategoryService _categoryService;

        private readonly Dictionary<string, string> _productSortingAliases = new Dictionary<string, string>();
        private readonly Dictionary<string, string> _categorySortingAliases = new Dictionary<string, string>();

        public CatalogSearchServiceImpl(Func<ICatalogRepository> catalogRepositoryFactory, IItemService itemService, ICatalogService catalogService, ICategoryService categoryService)
        {
            _catalogRepositoryFactory = catalogRepositoryFactory;
            _itemService = itemService;
            _catalogService = catalogService;
            _categoryService = categoryService;

            _productSortingAliases["sku"] = ReflectionUtility.GetPropertyName<CatalogProduct>(x => x.Code);
            _categorySortingAliases["sku"] = ReflectionUtility.GetPropertyName<Category>(x => x.Code);

        }

        public virtual SearchResult Search(SearchCriteria criteria)
        {
            var retVal = new SearchResult();
            var taskList = new List<Task>();

            if ((criteria.ResponseGroup & SearchResponseGroup.WithProducts) == SearchResponseGroup.WithProducts)
            {
                taskList.Add(Task.Factory.StartNew(() => SearchItems(criteria, retVal)));
            }

            if ((criteria.ResponseGroup & SearchResponseGroup.WithCatalogs) == SearchResponseGroup.WithCatalogs)
            {
                taskList.Add(Task.Factory.StartNew(() => SearchCatalogs(criteria, retVal)));
            }

            if ((criteria.ResponseGroup & SearchResponseGroup.WithCategories) == SearchResponseGroup.WithCategories)
            {
                taskList.Add(Task.Factory.StartNew(() => SearchCategories(criteria, retVal)));
            }

            Task.WaitAll(taskList.ToArray());

            return retVal;
        }


        protected virtual void SearchCategories(SearchCriteria criteria, SearchResult result)
        {
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
                        searchCategoryIds = searchCategoryIds.Concat(repository.GetAllChildrenCategoriesIds(searchCategoryIds)).ToArray();
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
                        searchCategoryIds = searchCategoryIds.Concat(repository.GetAllChildrenCategoriesIds(searchCategoryIds)).ToArray();
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

                var sortInfos = criteria.SortInfos;
                if (sortInfos.IsNullOrEmpty())
                {
                    sortInfos = new[] { new SortInfo { SortColumn = "Name" } };
                }
                //Try to replace sorting columns names
                TryTransformSortingInfoColumnNames(_categorySortingAliases, sortInfos);

                query = query.OrderBySortInfos(sortInfos);

                var categoryIds = query.Select(x => x.Id).ToList();
                var categoryResponseGroup = CategoryResponseGroup.Info | CategoryResponseGroup.WithImages | CategoryResponseGroup.WithSeo | CategoryResponseGroup.WithLinks | CategoryResponseGroup.WithParents;

                if (criteria.ResponseGroup.HasFlag(SearchResponseGroup.WithProperties))
                {
                    categoryResponseGroup |= CategoryResponseGroup.WithProperties;
                }

                if (criteria.ResponseGroup.HasFlag(SearchResponseGroup.WithOutlines))
                {
                    categoryResponseGroup |= CategoryResponseGroup.WithOutlines;
                }

                result.Categories = _categoryService.GetByIds(categoryIds.ToArray(), categoryResponseGroup, criteria.CatalogId)
                                                    .OrderBy(x => categoryIds.IndexOf(x.Id)).ToList();
            }
        }

        protected virtual void SearchCatalogs(SearchCriteria criteria, SearchResult result)
        {
            using (var repository = _catalogRepositoryFactory())
            {
                //Optimize performance and CPU usage
                repository.DisableChangesTracking();

                var catalogIds = criteria.CatalogIds;

                var sortInfos = criteria.SortInfos.IsNullOrEmpty()
                                    ? new[]
                                          {
                                              new SortInfo
                                                  {
                                                      SortColumn = "Name",
                                                      SortDirection = SortDirection.Ascending
                                                  }
                                          }
                                    : criteria.SortInfos;

                if (catalogIds.IsNullOrEmpty())
                {
                    catalogIds = repository.Catalogs.OrderBySortInfos(sortInfos).Select(x => x.Id).ToArray();
                }

                result.Catalogs = new List<Catalog>();

                foreach (var catalogId in catalogIds)
                {
                    var catalog = _catalogService.GetById(catalogId);

                    if (catalog != null)
                    {
                        result.Catalogs.Add(catalog);
                    }
                }
            }
        }

        protected virtual void SearchItems(SearchCriteria criteria, SearchResult result)
        {
            var sortInfos = criteria.SortInfos;
            if (sortInfos.IsNullOrEmpty())
            {
                sortInfos = new[] { new SortInfo { SortColumn = "Priority", SortDirection = SortDirection.Descending }, new SortInfo { SortColumn = "Name", SortDirection = SortDirection.Ascending } };
            }
            //Try to replace sorting columns names
            TryTransformSortingInfoColumnNames(_productSortingAliases, sortInfos);


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
                        searchCategoryIds = searchCategoryIds.Concat(repository.GetAllChildrenCategoriesIds(searchCategoryIds)).ToArray();
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
                var query = BuildSearchQuery(repository.Items, criteria, searchCategoryIds);

                result.ProductsTotalCount = query.Count();

                query = query.OrderBySortInfos(sortInfos);

                var itemIds = query.Skip(criteria.Skip)
                                   .Take(criteria.Take)
                                   .Select(x => x.Id)
                                   .ToList();

                var productResponseGroup = ItemResponseGroup.ItemInfo | ItemResponseGroup.ItemAssets | ItemResponseGroup.Links | ItemResponseGroup.Seo;

                if (criteria.ResponseGroup.HasFlag(SearchResponseGroup.WithProperties))
                {
                    productResponseGroup |= ItemResponseGroup.ItemProperties;
                }

                if (criteria.ResponseGroup.HasFlag(SearchResponseGroup.WithVariations))
                {
                    productResponseGroup |= ItemResponseGroup.Variations;
                }

                if (criteria.ResponseGroup.HasFlag(SearchResponseGroup.WithOutlines))
                {
                    productResponseGroup |= ItemResponseGroup.Outlines;
                }

                result.Products = _itemService.GetByIds(itemIds.ToArray(), productResponseGroup, criteria.CatalogId)
                                          .OrderBy(x => itemIds.IndexOf(x.Id)).ToList();
            }

        }

        protected virtual IQueryable<ItemEntity> BuildSearchQuery(IQueryable<ItemEntity> query, SearchCriteria criteria, string[] searchCategoryIds)
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

            //Filter by property dictionary values
            if (!criteria.PropertyValues.IsNullOrEmpty())
            {
                var propValueIds = criteria.PropertyValues.Select(x => x.ValueId).Distinct().ToArray();
                query = query.Where(x => x.ItemPropertyValues.Any(y => propValueIds.Contains(y.DictionaryItemId)));
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

        protected virtual void TryTransformSortingInfoColumnNames(IDictionary<string, string> transformationMap, SortInfo[] sortingInfos)
        {
            //Try to replace sorting columns names
            foreach (var sortInfo in sortingInfos)
            {
                string newColumnName;
                if (transformationMap.TryGetValue(sortInfo.SortColumn.ToLowerInvariant(), out newColumnName))
                {
                    sortInfo.SortColumn = newColumnName;
                }
            }
        }

    }
}
