using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using VirtoCommerce.Domain.Inventory.Model.Search;
using VirtoCommerce.InventoryModule.Core.Model;
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
        public InventorySearchService(Func<IInventoryRepository> repositoryFactory, IPlatformMemoryCache platformMemoryCache, IInventoryService inventoryService)
        {
            RepositoryFactory = repositoryFactory;
            PlatformMemoryCache = platformMemoryCache;
            InventoryService = inventoryService;
        }

        protected Func<IInventoryRepository> RepositoryFactory { get; }
        protected IPlatformMemoryCache PlatformMemoryCache { get; }
        protected IInventoryService InventoryService { get; }

        public virtual async Task<GenericSearchResult<InventoryInfo>> SearchInventoriesAsync(InventorySearchCriteria criteria)
        {
            var cacheKey = CacheKey.With(GetType(), "SearchInventoriesAsync", criteria.GetCacheKey());
            return await PlatformMemoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                cacheEntry.AddExpirationToken(InventorySearchCacheRegion.CreateChangeToken());
                var result = new GenericSearchResult<InventoryInfo>();
                using (var repository = RepositoryFactory())
                {
                    repository.DisableChangesTracking();

                    var sortInfos = GetSearchSortInfos(criteria);
                    var query = GetSearchQuery(repository, criteria, sortInfos);

                    result.TotalCount = await query.CountAsync();
                    if (criteria.Take > 0)
                    {
                        var inventoryIds = await query.Select(x => x.Id).Skip(criteria.Skip).Take(criteria.Take).ToArrayAsync();
                        result.Results = (await InventoryService.GetByIdsAsync(inventoryIds, criteria.ResponseGroup)).AsQueryable().OrderBySortInfos(sortInfos).ToArray();
                    }
                }
                return result;
            });
        }

        protected virtual IList<SortInfo> GetSearchSortInfos(InventorySearchCriteria criteria)
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

        protected virtual IQueryable<InventoryEntity> GetSearchQuery(IInventoryRepository repository, InventorySearchCriteria criteria, IEnumerable<SortInfo> sortInfos)
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
    }
}
