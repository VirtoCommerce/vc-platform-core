using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using VirtoCommerce.Domain.Inventory.Model.Search;
using VirtoCommerce.InventoryModule.Core.Model;
using VirtoCommerce.InventoryModule.Core.Services;
using VirtoCommerce.InventoryModule.Data.Caching;
using VirtoCommerce.InventoryModule.Data.Repositories;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Data.Infrastructure;

namespace VirtoCommerce.InventoryModule.Data.Services
{
    public class InventorySearchService : IInventorySearchService
    {
        private readonly Func<IInventoryRepository> _repositoryFactory;
        private readonly IPlatformMemoryCache _platformMemoryCache;
        private readonly IInventoryService _inventoryService;
        public InventorySearchService(Func<IInventoryRepository> repositoryFactory, IPlatformMemoryCache platformMemoryCache, IInventoryService inventoryService)
        {
            _repositoryFactory = repositoryFactory;
            _platformMemoryCache = platformMemoryCache;
            _inventoryService = inventoryService;
        }

        public async Task<GenericSearchResult<InventoryInfo>> SearchInventoriesAsync(InventorySearchCriteria criteria)
        {
            var cacheKey = CacheKey.With(GetType(), "SearchInventoriesAsync", criteria.GetCacheKey());
            return await _platformMemoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                cacheEntry.AddExpirationToken(InventorySearchCacheRegion.CreateChangeToken());
                var result = new GenericSearchResult<InventoryInfo>();
                using (var repository = _repositoryFactory())
                {
                    repository.DisableChangesTracking();

                    var query = repository.Inventories;
                    if (!criteria.ProductIds.IsNullOrEmpty())
                    {
                        query = query.Where(x => criteria.ProductIds.Contains(x.Sku));
                    }
                    if (!criteria.FulfillmentCenterIds.IsNullOrEmpty())
                    {
                        query = query.Where(x => criteria.FulfillmentCenterIds.Contains(x.FulfillmentCenterId));
                    }
                    var sortInfos = criteria.SortInfos;
                    if (sortInfos.IsNullOrEmpty())
                    {
                        sortInfos = new[]
                        {
                            new SortInfo
                            {
                                SortColumn = "ModifiedDate"
                            }
                        };
                    }

                    query = query.OrderBySortInfos(sortInfos);

                    result.TotalCount = await query.CountAsync();
                    if (criteria.Take > 0)
                    {
                        var inventoryIds = await query.Select(x => x.Id).Skip(criteria.Skip).Take(criteria.Take).ToArrayAsync();
                        result.Results = (await _inventoryService.GetByIdsAsync(inventoryIds, criteria.ResponseGroup)).AsQueryable().OrderBySortInfos(sortInfos).ToArray();
                    }
                }
                return result;
            });
        }
    }
}
