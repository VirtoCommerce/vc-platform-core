using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.Extensions.Caching.Memory;
using VirtoCommerce.CoreModule.Core.Payment;
using VirtoCommerce.CoreModule.Core.Seo;
using VirtoCommerce.CoreModule.Core.Shipping;
using VirtoCommerce.CoreModule.Core.Tax;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.DynamicProperties;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.Platform.Core.Settings;
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
        public StoreService(Func<IStoreRepository> repositoryFactory, ISeoService seoService, ISettingsManager settingManager,
                            IDynamicPropertyService dynamicPropertyService, IShippingMethodsRegistrar shippingService, IPaymentMethodsRegistrar paymentService,
                            ITaxProviderRegistrar taxService, IEventPublisher eventPublisher, IPlatformMemoryCache platformMemoryCache)
        {
            RepositoryFactory = repositoryFactory;
            SeoService = seoService;
            SettingManager = settingManager;
            DynamicPropertyService = dynamicPropertyService;
            ShippingMethodRegistrar = shippingService;
            PaymentMethodRegistrar = paymentService;
            TaxProviderRegistrar = taxService;
            EventPublisher = eventPublisher;
            PlatformMemoryCache = platformMemoryCache;
        }

        protected Func<IStoreRepository> RepositoryFactory { get; }
        protected ISeoService SeoService { get; }
        protected ISettingsManager SettingManager { get; }
        protected IDynamicPropertyService DynamicPropertyService { get; }
        protected IShippingMethodsRegistrar ShippingMethodRegistrar { get; }
        protected IPaymentMethodsRegistrar PaymentMethodRegistrar { get; }
        protected ITaxProviderRegistrar TaxProviderRegistrar { get; }
        protected IEventPublisher EventPublisher { get; }
        protected IPlatformMemoryCache PlatformMemoryCache { get; }

        #region IStoreService Members

        public virtual async Task<Store[]> GetByIdsAsync(string[] ids)
        {
            var cacheKey = CacheKey.With(GetType(), "GetByIdsAsync", string.Join("-", ids));
            return await PlatformMemoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                var stores = new List<Store>();

                using (var repository = RepositoryFactory())
                {
                    var dbStores = await repository.GetStoresByIdsAsync(ids);
                    foreach (var dbStore in dbStores)
                    {
                        var store = AbstractTypeFactory<Store>.TryCreateInstance();
                        dbStore.ToModel(store);

                        PopulateStore(store, dbStore);

                        await SettingManager.DeepLoadSettingsAsync(store);
                        stores.Add(store);
                        cacheEntry.AddExpirationToken(StoreCacheRegion.CreateChangeToken(store));
                    }
                }

                var result = stores.ToArray();
                var taskLoadDynamicPropertyValues = DynamicPropertyService.LoadDynamicPropertyValuesAsync(result);
                var taskLoadSeoForObjects = SeoService.LoadSeoForObjectsAsync(result);
                await Task.WhenAll(taskLoadDynamicPropertyValues, taskLoadSeoForObjects);

                return result;
            });
        }

        public virtual async Task<Store> GetByIdAsync(string id)
        {
            var stores = await GetByIdsAsync(new[] { id });
            return stores.FirstOrDefault();
        }

        public virtual async Task SaveChangesAsync(Store[] stores)
        {
            ValidateStoresProperties(stores);

            using (var repository = RepositoryFactory())
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

                    await repository.UnitOfWork.CommitAsync();
                    pkMap.ResolvePrimaryKeys();
                    await EventPublisher.Publish(new StoreChangedEvent(changedEntries));
                }
            }

            ClearCache(stores);
        }

        public virtual async Task DeleteAsync(string[] ids)
        {
            using (var repository = RepositoryFactory())
            {
                var changedEntries = new List<GenericChangedEntry<Store>>();
                var stores = await GetByIdsAsync(ids);
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
                await EventPublisher.Publish(new StoreChangedEvent(changedEntries));

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
                var stores = await GetByIdsAsync(new[] { user.StoreId });
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
            StoreSearchCacheRegion.ExpireRegion();

            foreach (var store in stores)
            {
                StoreCacheRegion.ExpireStore(store);
            }
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

        protected virtual void PopulateStore(Store store, StoreEntity dbStore)
        {
            //Return all registered methods with store settings 
            store.PaymentMethods = PaymentMethodRegistrar.GetAllPaymentMethods();
            foreach (var paymentMethod in store.PaymentMethods)
            {
                var dbStoredPaymentMethod = dbStore.PaymentMethods.FirstOrDefault(x => x.Code.EqualsInvariant(paymentMethod.Code));
                if (dbStoredPaymentMethod != null)
                {
                    dbStoredPaymentMethod.ToModel(paymentMethod);
                }
            }
            store.ShippingMethods = ShippingMethodRegistrar.GetAllShippingMethods();
            foreach (var shippingMethod in store.ShippingMethods)
            {
                var dbStoredShippingMethod = dbStore.ShippingMethods.FirstOrDefault(x => x.Code.EqualsInvariant(shippingMethod.Code));
                if (dbStoredShippingMethod != null)
                {
                    dbStoredShippingMethod.ToModel(shippingMethod);
                }
            }
            store.TaxProviders = TaxProviderRegistrar.GetAllTaxProviders();
            foreach (var taxProvider in store.TaxProviders)
            {
                var dbStoredTaxProvider = dbStore.TaxProviders.FirstOrDefault(x => x.Code.EqualsInvariant(taxProvider.Code));
                if (dbStoredTaxProvider != null)
                {
                    dbStoredTaxProvider.ToModel(taxProvider);
                }
            }
        }
        #endregion
    }
}
