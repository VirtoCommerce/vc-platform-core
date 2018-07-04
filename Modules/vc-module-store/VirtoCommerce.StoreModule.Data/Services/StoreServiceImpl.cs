using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.Extensions.Caching.Memory;
using VirtoCommerce.CoreModule.Core.Commerce.Model;
using VirtoCommerce.CoreModule.Core.Commerce.Services;
using VirtoCommerce.CoreModule.Core.Services;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.DynamicProperties;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.StoreModule.Core.Events;
using VirtoCommerce.StoreModule.Core.Model;
using VirtoCommerce.StoreModule.Core.Model.Search;
using VirtoCommerce.StoreModule.Core.Services;
using VirtoCommerce.StoreModule.Data.Model;
using VirtoCommerce.StoreModule.Data.Repositories;
using VirtoCommerce.StoreModule.Data.Services.Validation;

namespace VirtoCommerce.StoreModule.Data.Services
{
    public class StoreServiceImpl : IStoreService
    {
        private readonly Func<IStoreRepository> _repositoryFactory;
        private readonly ICommerceService _commerceService;
        private readonly ISettingsManager _settingManager;
        private readonly IDynamicPropertyService _dynamicPropertyService;
        private readonly IShippingMethodsRegistrar _shippingService;
        private readonly IPaymentMethodsRegistrar _paymentService;
        private readonly ITaxRegistrar _taxService;
        private readonly IEventPublisher _eventPublisher;
        private readonly IPlatformMemoryCache _platformMemoryCache;

        public StoreServiceImpl(Func<IStoreRepository> repositoryFactory, ICommerceService commerceService, ISettingsManager settingManager,
                                IDynamicPropertyService dynamicPropertyService, IShippingMethodsRegistrar shippingService, IPaymentMethodsRegistrar paymentService,
                                ITaxRegistrar taxService, IEventPublisher eventPublisher
            , IPlatformMemoryCache platformMemoryCache)
        {
            _repositoryFactory = repositoryFactory;
            _commerceService = commerceService;
            _settingManager = settingManager;
            _dynamicPropertyService = dynamicPropertyService;
            _shippingService = shippingService;
            _paymentService = paymentService;
            _taxService = taxService;
            _eventPublisher = eventPublisher;
            _platformMemoryCache = platformMemoryCache;
        }

        #region IStoreService Members

        public async Task<Store[]> GetByIdsAsync(string[] ids)
        {
            var cacheKey = CacheKey.With(GetType(), "GetByIdsAsync", string.Join("-", ids));
            return await _platformMemoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                var stores = new List<Store>();

                using (var repository = _repositoryFactory())
                {
                    var dbStores = await repository.GetStoresByIdsAsync(ids);
                    foreach (var dbStore in dbStores)
                    {
                        var store = AbstractTypeFactory<Store>.TryCreateInstance();
                        dbStore.ToModel(store);

                        PopulateStore(store, dbStore);

                        //Set default settings for store it can be override by store instance setting in LoadEntitySettingsValues
                        store.Settings = await _settingManager.GetModuleSettingsAsync("VirtoCommerce.Store");
                        await _settingManager.LoadEntitySettingsValuesAsync(store);
                        stores.Add(store);
                        cacheEntry.AddExpirationToken(StoreCacheRegion.CreateChangeToken(store));
                    }
                }

                var result = stores.ToArray();
                var taskLoadDynamicPropertyValues = _dynamicPropertyService.LoadDynamicPropertyValuesAsync(result);
                var taskLoadSeoForObjects = _commerceService.LoadSeoForObjectsAsync(result);
                await Task.WhenAll(taskLoadDynamicPropertyValues, taskLoadSeoForObjects);

                return result;
            });
        }

        public async Task<Store> GetByIdAsync(string id)
        {
            var stores = await GetByIdsAsync(new[] {id});
            return stores.FirstOrDefault();
        }

        public async Task SaveChangesAsync(Store[] stores)
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

                    await repository.UnitOfWork.CommitAsync();
                    pkMap.ResolvePrimaryKeys();
                    await _eventPublisher.Publish(new StoreChangedEvent(changedEntries));
                }
            }

            ClearCache(stores);
        }

        public async Task DeleteAsync(string[] ids)
        {
            using (var repository = _repositoryFactory())
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
                await _eventPublisher.Publish(new StoreChangedEvent(changedEntries));

                ClearCache(stores);
            }
        }

        /// <summary>
        /// Returns list of stores ids which passed user can signIn
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<string>> GetUserAllowedStoreIdsAsync(ApplicationUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            var retVal = new List<string>();

            if (user.StoreId != null)
            {
                var stores = await GetByIdsAsync(new []{ user.StoreId });
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

        private void ClearCache(IEnumerable<Store> stores)
        {
            foreach (var store in stores)
            {
                StoreCacheRegion.ExpireInventory(store);
            }
        }

        private void ValidateStoresProperties(IEnumerable<Store> stores)
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

        private void PopulateStore(Store store, StoreEntity dbStore)
        {
            //Return all registered methods with store settings 
            store.PaymentMethods = _paymentService.GetAllPaymentMethods();
            foreach (var paymentMethod in store.PaymentMethods)
            {
                var dbStoredPaymentMethod =
                    dbStore.PaymentMethods.FirstOrDefault(x => x.Code.EqualsInvariant(paymentMethod.Code));
                if (dbStoredPaymentMethod != null)
                {
                    dbStoredPaymentMethod.ToModel(paymentMethod);
                }
            }
            store.ShippingMethods = _shippingService.GetAllShippingMethods();
            foreach (var shippingMethod in store.ShippingMethods)
            {
                var dbStoredShippingMethod =
                    dbStore.ShippingMethods.FirstOrDefault(x =>
                        x.Code.EqualsInvariant(shippingMethod.Code));
                if (dbStoredShippingMethod != null)
                {
                    dbStoredShippingMethod.ToModel(shippingMethod);
                }
            }
            store.TaxProviders = _taxService.GetAllTaxProviders();
            foreach (var taxProvider in store.TaxProviders)
            {
                var dbStoredTaxProvider =
                    dbStore.TaxProviders.FirstOrDefault(x => x.Code.EqualsInvariant(taxProvider.Code));
                if (dbStoredTaxProvider != null)
                {
                    dbStoredTaxProvider.ToModel(taxProvider);
                }
            }
        }

        #endregion
    }
}
