using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using VirtoCommerce.Domain.Inventory.Model.Search;
using VirtoCommerce.InventoryModule.Core.Model.Search;
using VirtoCommerce.InventoryModule.Core.Services;
using VirtoCommerce.InventoryModule.Data.Caching;
using VirtoCommerce.InventoryModule.Data.Model;
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

        public virtual async Task<InventoryInfoSearchResult> SearchInventoriesAsync(InventorySearchCriteria criteria)
        {
            var cacheKey = CacheKey.With(GetType(), "SearchInventoriesAsync", criteria.GetCacheKey());
            return await _platformMemoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                cacheEntry.AddExpirationToken(InventorySearchCacheRegion.CreateChangeToken());
                var result = AbstractTypeFactory<InventoryInfoSearchResult>.TryCreateInstance();
                using (var repository = _repositoryFactory())
                {
                    repository.DisableChangesTracking();

                    var sortInfos = GetSortInfos(criteria);
                    var query = GetQuery(repository, criteria, sortInfos);

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

        protected virtual IQueryable<InventoryEntity> GetQuery(IInventoryRepository repository,
            InventorySearchCriteria criteria, IEnumerable<SortInfo> sortInfos)
        {
            var query = repository.Inventories;
            if (!criteria.ProductIds.IsNullOrEmpty())
            {
                query = query.Where(x => criteria.ProductIds.Contains(x.Sku));
            }
            if (!criteria.FulfillmentCenterIds.IsNullOrEmpty())
            {
                query = query.Where(x => criteria.FulfillmentCenterIds.Contains(x.FulfillmentCenterId));
            }

            return query.OrderBySortInfos(sortInfos);
        }

        protected virtual IList<SortInfo> GetSortInfos(InventorySearchCriteria criteria)
        {
            var sortInfos = criteria.SortInfos;
            if (sortInfos.IsNullOrEmpty())
            {
                sortInfos = new[]
                {
                    new SortInfo { SortColumn = "ModifiedDate" }
                };
            }

            return sortInfos;
        }
    }
}
