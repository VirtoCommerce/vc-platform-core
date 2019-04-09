using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.CatalogModule.Core.Model.Search;
using VirtoCommerce.CatalogModule.Core.Search;
using VirtoCommerce.CatalogModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.ExportImport;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.SearchModule.Core.Model;
using VirtoCommerce.SitemapsModule.Core.Models;
using VirtoCommerce.SitemapsModule.Core.Services;
using VirtoCommerce.StoreModule.Core.Model;

namespace VirtoCommerce.SitemapsModule.Data.Services.SitemapItemRecordProviders
{
    public class CatalogSitemapItemRecordProvider : SitemapItemRecordProviderBase, ISitemapItemRecordProvider
    {
        public CatalogSitemapItemRecordProvider(ISettingsManager settingsManager, ISitemapUrlBuilder urlBuilider)
            : base(settingsManager, urlBuilider)
        {
        }

        public CatalogSitemapItemRecordProvider(
            ICategoryService categoryService,
            IItemService itemService,
            IListEntrySearchService listEntrySearchService,
            ISitemapUrlBuilder urlBuilder,
            ISettingsManager settingsManager)
            : base(settingsManager, urlBuilder)
        {
            CategoryService = categoryService;
            ItemService = itemService;
            ListEntrySearchService = listEntrySearchService;
        }

        protected ICategoryService CategoryService { get; }
        protected IItemService ItemService { get; }
        protected IListEntrySearchService ListEntrySearchService { get; }

        public virtual async Task LoadSitemapItemRecordsAsync(Store store, Sitemap sitemap, string baseUrl, Action<ExportImportProgressInfo> progressCallback = null)
        {
            await ProcesseProductSitemapItem(store, sitemap, baseUrl);
            await ProcesseCategorySitemapItems(store, sitemap, baseUrl, progressCallback);
        }

        private async Task ProcesseProductSitemapItem(Store store, Sitemap sitemap, string baseUrl)
        {
            var productOptions = GetProductOptions();
            var productSitemapItems = sitemap.Items.Where(si => si.ObjectType.EqualsInvariant(SitemapItemTypes.Product));
            var productIds = productSitemapItems.Select(si => si.ObjectId).ToArray();
            var itemResponceGroup = (ItemResponseGroup.Seo | ItemResponseGroup.Outlines).ToString();

            var products = (await ItemService.GetByIdsAsync(productIds, itemResponceGroup)).Where(p => !p.IsActive.HasValue || p.IsActive.Value);

            foreach (var sitemapItem in productSitemapItems)
            {
                var product = products.FirstOrDefault(x => x.Id == sitemapItem.ObjectId);
                if (product == null)
                {
                    continue;
                }
                var itemRecords = GetSitemapItemRecords(store, productOptions, sitemap.UrlTemplate, baseUrl, product);
                sitemapItem.ItemsRecords.AddRange(itemRecords);
            }
        }

        private async Task ProcesseCategorySitemapItems(Store store, Sitemap sitemap, string baseUrl, Action<ExportImportProgressInfo> progressCallback)
        {
            var productOptions = GetProductOptions();
            var searchBunchSize = SettingsManager.GetValue("Sitemap.SearchBunchSize", 500);

            var categorySitemapItems = sitemap.Items.Where(x => x.ObjectType.EqualsInvariant(SitemapItemTypes.Category));
            var categoryIds = categorySitemapItems.Select(x => x.ObjectId).ToArray();
            var categoryResponceGroup = (CategoryResponseGroup.WithSeo | CategoryResponseGroup.WithOutlines).ToString();
            var categories = (await CategoryService.GetByIdsAsync(categoryIds, categoryResponceGroup))
                                                   .Where(c => !c.IsActive.HasValue || c.IsActive.Value);

            var progressInfo = GetProgressInfo(progressCallback, categories.Count());
            progressInfo.Start();

            foreach (var sitemapItem in categorySitemapItems)
            {
                var category = categories.FirstOrDefault(x => x.Id == sitemapItem.ObjectId);
                if (category == null) continue;

                var productsItemRecords = await GetProductItemRecordsForCategory(store, sitemap, baseUrl, productOptions, searchBunchSize, category);
                var categoryItemRecords = await GetCategorySitemapItems(store, sitemap, baseUrl, searchBunchSize, category);

                sitemapItem.ItemsRecords.AddRange(categoryItemRecords);
                sitemapItem.ItemsRecords.AddRange(productsItemRecords);

                progressInfo.Next();
            }
        }

        private async Task<ConcurrentBag<SitemapItemRecord>> GetProductItemRecordsForCategory(Store store, Sitemap sitemap, string baseUrl, SitemapItemOptions productOptions, int searchBunchSize, Category category)
        {
            //Load all category products
            var productTotalCount = await GetTotalProductCount(category);

            var itemRecords = new ConcurrentBag<SitemapItemRecord>();
            Parallel.For(0, (int)Math.Ceiling(productTotalCount / (double)searchBunchSize), new ParallelOptions { MaxDegreeOfParallelism = 1 }, (i) =>
            {
                var productSearchCriteria = new CatalogListEntrySearchCriteria
                {
                    CategoryId = category.Id,
                    ResponseGroup = (CategoryResponseGroup.WithSeo).ToString(), // проверить с WithCategories - ответ должен быть тотже
                    Skip = i * searchBunchSize,
                    Take = searchBunchSize,
                    HideDirectLinkedCategories = true,
                    SearchInChildren = true,
                    OnlyBuyable = true,
                    ObjectType = KnownDocumentTypes.Product
                };

                var productSearchResult = ListEntrySearchService.SearchAsync(productSearchCriteria).Result;
                var productIds = productSearchResult.ListEntries.Select(si => si.Id).ToArray();
                var products = (ItemService.GetByIdsAsync(productIds, ItemResponseGroup.WithSeo.ToString()).Result).Where(p => !p.IsActive.HasValue || p.IsActive.Value);

                foreach (var product in products.Where(p => !p.IsActive.HasValue || p.IsActive.Value))
                {
                    foreach (var record in GetSitemapItemRecords(store, productOptions, sitemap.UrlTemplate, baseUrl, product))
                    {
                        itemRecords.Add(record);
                    }
                }
            });
            return itemRecords;
        }

        private async Task<ICollection<SitemapItemRecord>> GetCategorySitemapItems(Store store, Sitemap sitemap, string baseUrl, int searchBunchSize, Category category)
        {
            var categoryOptions = GetCategoryOptions();
            var result = GetSitemapItemRecords(store, categoryOptions, sitemap.UrlTemplate, baseUrl, category);
            var responseGroup = (CategoryResponseGroup.WithSeo | CategoryResponseGroup.WithOutlines).ToString();

            var catalogSearchCriteria = new CatalogListEntrySearchCriteria
            {
                CategoryId = category.Id,
                ResponseGroup = responseGroup, // проверить с WithCategories - ответ должен быть тотже
                Skip = 0,
                Take = searchBunchSize,
                HideDirectLinkedCategories = true,
                SearchInChildren = true,
                ObjectType = KnownDocumentTypes.Category
            };

            var catalogSearchResult = await ListEntrySearchService.SearchAsync(catalogSearchCriteria);

            var categoryIds = catalogSearchResult.ListEntries.Select(x => x.Id).ToArray();
            var categories = (await CategoryService.GetByIdsAsync(categoryIds, responseGroup))
                .Where(c => !c.IsActive.HasValue || c.IsActive.Value);

            foreach (var seoObj in categories)
            {
                result.AddRange(GetSitemapItemRecords(store, categoryOptions, sitemap.UrlTemplate, baseUrl, seoObj));
            }

            return result;
        }

        private async Task<int> GetTotalProductCount(Category category)
        {
            var catalogSearchCriteria = new CatalogListEntrySearchCriteria
            {
                CategoryId = category.Id,
                ResponseGroup = ItemResponseGroup.None.ToString(),
                Skip = 0,
                Take = 1,
                HideDirectLinkedCategories = true,
                SearchInChildren = true,
                ObjectType = KnownDocumentTypes.Product
            };

            var productTotalCount = (await ListEntrySearchService.SearchAsync(catalogSearchCriteria)).TotalCount;
            return productTotalCount;
        }

        private SitemapItemOptions GetProductOptions()
        {
            return new SitemapItemOptions
            {
                Priority = SettingsManager.GetValue("Sitemap.ProductPagePriority", 1.0M),
                UpdateFrequency = SettingsManager.GetValue("Sitemap.ProductPageUpdateFrequency", UpdateFrequency.Daily)
            };
        }

        private SitemapItemOptions GetCategoryOptions()
        {
            return new SitemapItemOptions
            {
                Priority = SettingsManager.GetValue("Sitemap.CategoryPagePriority", .7M),
                UpdateFrequency = SettingsManager.GetValue("Sitemap.CategoryPageUpdateFrequency", UpdateFrequency.Weekly)
            };
        }

        private SitemapProgressInfo GetProgressInfo(Action<ExportImportProgressInfo> progressCallback, long totalCount)
        {
            return new SitemapProgressInfo
            {
                StartDescriptionTemplate = "Catalog: start generating records for {0} categories",
                EndDescriptionTemplate = "Catalog:  {0} categories generated",
                ProgressDescriptionTemplate = "Catalog: generated records for {0} of {1} categories",
                ProgressCallback = progressCallback,
                TotalCount = totalCount
            };
        }
    }
}
