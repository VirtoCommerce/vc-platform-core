using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.Extensions.Caching.Memory;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.DynamicProperties;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.Platform.Data.Infrastructure;
using VirtoCommerce.StoreModule.Core.Events;
using VirtoCommerce.StoreModule.Core.Model;
using VirtoCommerce.StoreModule.Core.Services;
using VirtoCommerce.StoreModule.Data.Caching;
using VirtoCommerce.StoreModule.Data.Model;
using VirtoCommerce.StoreModule.Data.Repositories;
using VirtoCommerce.StoreModule.Data.Services.Validation;

namespace VirtoCommerce.StoreModule.Data.Services
{
    public class StoreService : IStoreService
    {
        private readonly Func<IStoreRepository> _repositoryFactory;
        private readonly ISettingsManager _settingManager;
        private readonly IEventPublisher _eventPublisher;
        private readonly IPlatformMemoryCache _platformMemoryCache;

        public StoreService(Func<IStoreRepository> repositoryFactory, ISettingsManager settingManager, IEventPublisher eventPublisher, IPlatformMemoryCache platformMemoryCache)
        {
            _repositoryFactory = repositoryFactory;
            _settingManager = settingManager;

            _eventPublisher = eventPublisher;
            _platformMemoryCache = platformMemoryCache;
        }

        #region IStoreService Members

        public virtual async Task<Store[]> GetByIdsAsync(string[] ids, string responseGroup = null)
        {
            var cacheKey = CacheKey.With(GetType(), "GetByIdsAsync", string.Join("-", ids), responseGroup);
            return await _platformMemoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                var stores = new List<Store>();

                cacheEntry.AddExpirationToken(StoreCacheRegion.CreateChangeToken());

                using (var repository = _repositoryFactory())
                {
                    repository.DisableChangesTracking();

                    var dbStores = await repository.GetStoresByIdsAsync(ids, responseGroup);

                    foreach (var dbStore in dbStores)
                    {
                        var store = AbstractTypeFactory<Store>.TryCreateInstance();
                        dbStore.ToModel(store);

                        await _settingManager.DeepLoadSettingsAsync(store);
                        stores.Add(store);
                    }
                }

                return stores.ToArray();
            });
        }

        public virtual async Task<Store> GetByIdAsync(string id, string responseGroup = null)
        {
            var stores = await GetByIdsAsync(new[] { id }, responseGroup);
            return stores.FirstOrDefault();
        }

        public virtual async Task SaveChangesAsync(Store[] stores)
        {
            ValidateStoresProperties(stores);

            using (var repository = _repositoryFactory())
            {
                var changedEntries = new List<GenericChangedEntry<Store>>();
                var pkMap = new PrimaryKeyResolvingMap();
                var dbStores = await repository.GetStoresByIdsAsync(stores.Select(x => x.Id).ToArray());

                foreach (var store in stores)
                {
                    var targetEntity = dbStores.FirstOrDefault(x => x.Id == store.Id);
                    var sourceEntity = AbstractTypeFactory<StoreEntity>.TryCreateInstance().FromModel(store, pkMap);

                    if (targetEntity != null)
                    {
                        changedEntries.Add(new GenericChangedEntry<Store>(store, targetEntity.ToModel(AbstractTypeFactory<Store>.TryCreateInstance()),
                            EntryState.Modified));
                        sourceEntity.Patch(targetEntity);
                    }
                    else
                    {
                        repository.Add(sourceEntity);
                        changedEntries.Add(new GenericChangedEntry<Store>(store, EntryState.Added));
                    }
                }

                await repository.UnitOfWork.CommitAsync();
                pkMap.ResolvePrimaryKeys();
                await _eventPublisher.Publish(new StoreChangedEvent(changedEntries));
            }

            ClearCache(stores);
        }

        public virtual async Task DeleteAsync(string[] ids)
        {
            using (var repository = _repositoryFactory())
            {
                var changedEntries = new List<GenericChangedEntry<Store>>();
                var stores = await GetByIdsAsync(ids, StoreResponseGroup.StoreInfo.ToString());
                var dbStores = await repository.GetStoresByIdsAsync(ids);

                foreach (var store in stores)
                {
                    var dbStore = dbStores.FirstOrDefault(x => x.Id == store.Id);
                    if (dbStore != null)
                    {
                        repository.Remove(dbStore);
                        changedEntries.Add(new GenericChangedEntry<Store>(store, EntryState.Deleted));
                    }
                }
                await repository.UnitOfWork.CommitAsync();
                await _eventPublisher.Publish(new StoreChangedEvent(changedEntries));

                ClearCache(stores);
            }
        }

        /// <summary>
        /// Returns list of stores ids which passed user can signIn
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public virtual async Task<IEnumerable<string>> GetUserAllowedStoreIdsAsync(ApplicationUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            var retVal = new List<string>();

            if (user.StoreId != null)
            {
                var stores = await GetByIdsAsync(new[] { user.StoreId }, StoreResponseGroup.StoreInfo.ToString());
                foreach (var store in stores)
                {
                    retVal.Add(store.Id);
                    if (!store.TrustedGroups.IsNullOrEmpty())
                    {
                        retVal.AddRange(store.TrustedGroups);
                    }
                }
            }
            return retVal;
        }

        protected virtual void ClearCache(IEnumerable<Store> stores)
        {
            StoreCacheRegion.ExpireRegion();
        }

        protected virtual void ValidateStoresProperties(IEnumerable<Store> stores)
        {
            if (stores == null)
            {
                throw new ArgumentNullException(nameof(stores));
            }

            var validator = new StoreValidator();
            foreach (var store in stores)
            {
                validator.ValidateAndThrow(store);
            }
        }



        #endregion
    }
}
