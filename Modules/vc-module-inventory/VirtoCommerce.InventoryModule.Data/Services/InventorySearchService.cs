using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using VirtoCommerce.Domain.Inventory.Model.Search;
using VirtoCommerce.InventoryModule.Core.Model;
using VirtoCommerce.InventoryModule.Core.Services;
using VirtoCommerce.InventoryModule.Data.Cashing;
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
        public InventorySearchService(Func<IInventoryRepository> repositoryFactory, IPlatformMemoryCache platformMemoryCache)
        {
            _repositoryFactory = repositoryFactory;
            _platformMemoryCache = platformMemoryCache;
        }

        public async Task<GenericSearchResult<InventoryInfo>> SearchInventoriesAsync(InventorySearchCriteria criteria)
        {
            var cacheKey = CacheKey.With(GetType(), "SearchInventoriesAsync", criteria.GetCacheKey());
            return await _platformMemoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
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
                    var list = await query.Skip(criteria.Skip).Take(criteria.Take).ToArrayAsync();
                    result.Results = list
                        .Select(x =>
                        {
                            var inventory = x.ToModel(AbstractTypeFactory<InventoryInfo>.TryCreateInstance());
                            cacheEntry.AddExpirationToken(InventoryCacheRegion.CreateChangeToken(inventory));
                            return inventory;
                        })
                        .ToList();
                }
                return result;
            });
        }
    }
}
