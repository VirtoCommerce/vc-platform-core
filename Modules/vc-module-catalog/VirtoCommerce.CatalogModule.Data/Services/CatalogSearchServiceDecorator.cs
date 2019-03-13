using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Caching.Memory;
using VirtoCommerce.CatalogModule.Core;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.CatalogModule.Core.Model.Search;
using VirtoCommerce.CatalogModule.Core.Search;
using VirtoCommerce.CatalogModule.Core.Services;
using VirtoCommerce.CatalogModule.Data.Caching;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.SearchModule.Core.Model;

namespace VirtoCommerce.CatalogModule.Data.Services
{
    /// <summary>
    /// Another implementation for ICatalogSearchService. Combines indexed and DB search providers.
    /// </summary>
    public class CatalogSearchServiceDecorator : ICatalogSearchService
    {
        private readonly CatalogSearchServiceImpl _catalogSearchService;
        private readonly IItemService _itemService;
        private readonly IProductSearchService _productSearchService;
        private readonly ISettingsManager _settingsManager;
        private readonly IPlatformMemoryCache _platformMemoryCache;

        public CatalogSearchServiceDecorator(
            CatalogSearchServiceImpl catalogSearchService,
            IItemService itemService,
            IProductSearchService productSearchService,
            ISettingsManager settingsManager, IPlatformMemoryCache platformMemoryCache)
        {
            _catalogSearchService = catalogSearchService;
            _itemService = itemService;
            _productSearchService = productSearchService;
            _settingsManager = settingsManager;
            _platformMemoryCache = platformMemoryCache;
        }

        public virtual async Task<SearchResult> SearchAsync(CatalogListEntrySearchCriteria criteria)
        {
            var cacheKey = CacheKey.With(GetType(), "SearchAsync", criteria.GetCacheKey());
            return await _platformMemoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                AddExpirationToken(cacheEntry, criteria.ResponseGroup);

                SearchResult result;

                var useIndexedSearch =
                    _settingsManager.GetValue(ModuleConstants.Settings.Search.UseCatalogIndexedSearchInManager.Name,
                        true);
                var searchProducts = criteria.ResponseGroup.HasFlag(SearchResponseGroup.WithProducts);

                if (useIndexedSearch && searchProducts && !string.IsNullOrEmpty(criteria.Keyword))
                {
                    result = new SearchResult();

                    // TODO: create outline for category
                    // TODO: implement sorting

                    const ItemResponseGroup responseGroup = ItemResponseGroup.ItemInfo | ItemResponseGroup.Outlines;

                    var serviceCriteria = AbstractTypeFactory<ProductSearchCriteria>.TryCreateInstance();

                    serviceCriteria.ObjectType = KnownDocumentTypes.Product;
                    serviceCriteria.Keyword = criteria.Keyword;
                    serviceCriteria.CatalogId = criteria.CatalogId;
                    serviceCriteria.Outline = criteria.CategoryId;
                    serviceCriteria.WithHidden = criteria.WithHidden;
                    serviceCriteria.Skip = criteria.Skip;
                    serviceCriteria.Take = criteria.Take;
                    serviceCriteria.ResponseGroup = responseGroup.ToString();
                    serviceCriteria.Sort = criteria.Sort;

                    await SearchItemsAsync(result, serviceCriteria, responseGroup);
                }
                else
                {
                    // use original implementation from catalog module
                    result = await _catalogSearchService.SearchAsync(criteria);
                }

                return result;
            });
        }

        protected virtual async Task SearchItemsAsync(SearchResult result, ProductSearchCriteria criteria, ItemResponseGroup responseGroup)
        {
            // Search using criteria, it will only return IDs of the items
            var searchResults = await _productSearchService.SearchAsync(criteria);

            result.ProductsTotalCount = (int)searchResults.TotalCount;

            if (!searchResults.Items.IsNullOrEmpty())
            {
                // Now load items from repository preserving original order
                var itemIds = searchResults.Items.Select(i => i.Id).ToArray();
                result.Products = (await _itemService.GetByIdsAsync(itemIds, responseGroup, criteria.CatalogId))
                    .OrderBy(i => itemIds.IndexOf(i.Id)).ToArray();
            }
        }

        private void AddExpirationToken(MemoryCacheEntryOptions cacheEntry, SearchResponseGroup searchResponseGroup)
        {
            if (searchResponseGroup.HasFlag(SearchResponseGroup.WithCatalogs))
            {
                cacheEntry.AddExpirationToken(CatalogCacheRegion.CreateChangeToken());
            }

            if (searchResponseGroup.HasFlag(SearchResponseGroup.WithCategories))
            {
                cacheEntry.AddExpirationToken(CategoryCacheRegion.CreateChangeToken());
            }

            if (searchResponseGroup.HasFlag(SearchResponseGroup.WithProducts))
            {
                cacheEntry.AddExpirationToken(ItemSearchCacheRegion.CreateChangeToken());
            }
        }
    }
}
