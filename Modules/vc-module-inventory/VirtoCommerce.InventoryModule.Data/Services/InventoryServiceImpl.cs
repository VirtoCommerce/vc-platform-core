using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using VirtoCommerce.InventoryModule.Core.Events;
using VirtoCommerce.InventoryModule.Core.Model;
using VirtoCommerce.InventoryModule.Core.Services;
using VirtoCommerce.InventoryModule.Data.Caching;
using VirtoCommerce.InventoryModule.Data.Model;
using VirtoCommerce.InventoryModule.Data.Repositories;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.Platform.Data.Infrastructure;

namespace VirtoCommerce.InventoryModule.Data.Services
{
    public class InventoryServiceImpl : IInventoryService
    {
        private readonly Func<IInventoryRepository> _repositoryFactory;
        private readonly IEventPublisher _eventPublisher;
        private readonly IPlatformMemoryCache _platformMemoryCache;

        public InventoryServiceImpl(Func<IInventoryRepository> repositoryFactory, IEventPublisher eventPublisher, IPlatformMemoryCache platformMemoryCache)
        {
            _repositoryFactory = repositoryFactory;
            _eventPublisher = eventPublisher;
            _platformMemoryCache = platformMemoryCache;
        }

        public virtual async Task<IEnumerable<InventoryInfo>> GetByIdsAsync(string[] itemIds, string responseGroup = null)
        {
            var cacheKey = CacheKey.With(GetType(), "GetByIdsAsync", string.Join("-", itemIds), responseGroup);
            return await _platformMemoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                using (var repository = _repositoryFactory())
                {
                    repository.DisableChangesTracking();
                    var entries = await repository.Inventories.ToArrayAsync();

                    return entries.Select(e =>
                    {
                        var result = e.ToModel(AbstractTypeFactory<InventoryInfo>.TryCreateInstance());
                        cacheEntry.AddExpirationToken(InventoryCacheRegion.CreateChangeToken(result));
                        return result;
                    }).ToArray();
                }
            });
        }

        #region IInventoryService Members

        public virtual async Task<IEnumerable<InventoryInfo>> GetProductsInventoryInfosAsync(IEnumerable<string> productIds, string responseGroup = null)
        {
            var cacheKey = CacheKey.With(GetType(), "GetProductsInventoryInfosAsync", string.Join("-", productIds), responseGroup);
            return await _platformMemoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                var retVal = new List<InventoryInfo>();
                using (var repository = _repositoryFactory())
                {
                    repository.DisableChangesTracking();
                    var entities = await repository.GetProductsInventoriesAsync(productIds.ToArray(), responseGroup);
                    retVal.AddRange(entities.Select(x =>
                    {
                        var result = x.ToModel(AbstractTypeFactory<InventoryInfo>.TryCreateInstance());
                        cacheEntry.AddExpirationToken(InventoryCacheRegion.CreateChangeToken(result));
                        return result;
                    }));
                }
                return retVal;
            });
        }

        public virtual async Task SaveChangesAsync(IEnumerable<InventoryInfo> inventoryInfos)
        {
            if (inventoryInfos == null)
            {
                throw new ArgumentNullException(nameof(inventoryInfos));
            }

            var changedEntries = new List<GenericChangedEntry<InventoryInfo>>();
            using (var repository = _repositoryFactory())
            {
                var dataExistInventories = await repository.GetProductsInventoriesAsync(inventoryInfos.Select(x => x.ProductId));
                foreach (var changedInventory in inventoryInfos)
                {
                    var originalEntity = dataExistInventories.FirstOrDefault(x => x.Sku == changedInventory.ProductId && x.FulfillmentCenterId == changedInventory.FulfillmentCenterId);

                    var modifiedEntity = AbstractTypeFactory<InventoryEntity>.TryCreateInstance().FromModel(changedInventory);
                    if (originalEntity != null)
                    {
                        changedEntries.Add(new GenericChangedEntry<InventoryInfo>(changedInventory, originalEntity.ToModel(AbstractTypeFactory<InventoryInfo>.TryCreateInstance()), EntryState.Modified));
                        modifiedEntity?.Patch(originalEntity);
                    }
                    else
                    {
                        repository.Add(modifiedEntity);
                        changedEntries.Add(new GenericChangedEntry<InventoryInfo>(changedInventory, EntryState.Added));
                    }
                }

                //Raise domain events
                await _eventPublisher.Publish(new InventoryChangingEvent(changedEntries));
                await repository.UnitOfWork.CommitAsync();
                await _eventPublisher.Publish(new InventoryChangedEvent(changedEntries));

                ClearCache(inventoryInfos);
            }
        }

        protected virtual void ClearCache(IEnumerable<InventoryInfo> inventories)
        {
            InventorySearchCacheRegion.ExpireRegion();

            foreach (var inventory in inventories)
            {
                InventoryCacheRegion.ExpireInventory(inventory);
            }
        }

        #endregion

    }
}
