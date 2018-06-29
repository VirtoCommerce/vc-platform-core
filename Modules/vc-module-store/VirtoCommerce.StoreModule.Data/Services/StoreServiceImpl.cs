using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using VirtoCommerce.CoreModule.Core.Commerce.Model;
using VirtoCommerce.CoreModule.Core.Commerce.Services;
using VirtoCommerce.CoreModule.Core.Services;
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

        public StoreServiceImpl(Func<IStoreRepository> repositoryFactory, ICommerceService commerceService, ISettingsManager settingManager,
                                IDynamicPropertyService dynamicPropertyService, IShippingMethodsRegistrar shippingService, IPaymentMethodsRegistrar paymentService,
                                ITaxRegistrar taxService, IEventPublisher eventPublisher)
        {
            _repositoryFactory = repositoryFactory;
            _commerceService = commerceService;
            _settingManager = settingManager;
            _dynamicPropertyService = dynamicPropertyService;
            _shippingService = shippingService;
            _paymentService = paymentService;
            _taxService = taxService;
            _eventPublisher = eventPublisher;
        }

        #region IStoreService Members

        public async Task<Store[]> GetByIdsAsync(string[] ids)
        {
            var stores = new List<Store>();

            using (var repository = _repositoryFactory())
            {               
                var dbStores = await repository.GetStoresByIdsAsync(ids);
                foreach (var dbStore in dbStores)
                {
                    var store = AbstractTypeFactory<Store>.TryCreateInstance();
                    dbStore.ToModel(store);

                    //Return all registered methods with store settings 
                    store.PaymentMethods = _paymentService.GetAllPaymentMethods();
                    foreach (var paymentMethod in store.PaymentMethods)
                    {
                        var dbStoredPaymentMethod = dbStore.PaymentMethods.FirstOrDefault(x => x.Code.EqualsInvariant(paymentMethod.Code));
                        if (dbStoredPaymentMethod != null)
                        {
                            dbStoredPaymentMethod.ToModel(paymentMethod);
                        }
                    }
                    store.ShippingMethods = _shippingService.GetAllShippingMethods();
                    foreach (var shippingMethod in store.ShippingMethods)
                    {
                        var dbStoredShippingMethod = dbStore.ShippingMethods.FirstOrDefault(x => x.Code.EqualsInvariant(shippingMethod.Code));
                        if (dbStoredShippingMethod != null)
                        {
                            dbStoredShippingMethod.ToModel(shippingMethod);
                        }
                    }
                    store.TaxProviders = _taxService.GetAllTaxProviders();
                    foreach (var taxProvider in store.TaxProviders)
                    {
                        var dbStoredTaxProvider = dbStore.TaxProviders.FirstOrDefault(x => x.Code.EqualsInvariant(taxProvider.Code));
                        if (dbStoredTaxProvider != null)
                        {
                            dbStoredTaxProvider.ToModel(taxProvider);
                        }
                    }

                    //Set default settings for store it can be override by store instance setting in LoadEntitySettingsValues
                    store.Settings = await _settingManager.GetModuleSettingsAsync("VirtoCommerce.Store");
                    await _settingManager.LoadEntitySettingsValuesAsync(store);
                    stores.Add(store);
                }
            }

            var result = stores.ToArray();
            await _dynamicPropertyService.LoadDynamicPropertyValuesAsync(result);
            _commerceService.LoadSeoForObjects(result);
            return result;
        }

        public async Task<Store> GetByIdAsync(string id)
        {
            var entities = await GetByIdsAsync(new[] {id});
            return entities.FirstOrDefault();
        }

        public async Task<Store> CreateAsync(Store store)
        {
            var pkMap = new PrimaryKeyResolvingMap();

            ValidateStoreProperties(store);

            var changedEntries = new List<GenericChangedEntry<Store>>();

            var dbStore = AbstractTypeFactory<StoreEntity>.TryCreateInstance();
            dbStore = dbStore.FromModel(store, pkMap);

            using (var repository = _repositoryFactory())
            {
                repository.Add(dbStore);
                await repository.UnitOfWork.CommitAsync();
                pkMap.ResolvePrimaryKeys();

                changedEntries.Add(new GenericChangedEntry<Store>(store, EntryState.Added));
                await _eventPublisher.Publish(new StoreChangedEvent(changedEntries));
            }

            return store;
        }

        public async Task UpdateAsync(Store[] stores)
        {
            var pkMap = new PrimaryKeyResolvingMap();
            var changedEntries = new List<GenericChangedEntry<Store>>();

            using (var repository = _repositoryFactory())
            {
                var dbStores = await repository.GetStoresByIdsAsync(stores.Select(x => x.Id).ToArray());
                foreach (var store in stores)
                {
                    var sourceEntity = AbstractTypeFactory<StoreEntity>.TryCreateInstance().FromModel(store, pkMap);
                    var targetEntity = dbStores.First(x => x.Id == store.Id);

                    if (targetEntity != null)
                    {
                        changedEntries.Add(new GenericChangedEntry<Store>(store, targetEntity.ToModel(AbstractTypeFactory<Store>.TryCreateInstance()),
                            EntryState.Modified));
                        sourceEntity.Patch(targetEntity);
                    }

                    await repository.UnitOfWork.CommitAsync();
                }
                await _eventPublisher.Publish(new StoreChangedEvent(changedEntries));
            }
        }

        public async Task DeleteAsync(string[] ids)
        {
            using (var repository = _repositoryFactory())
            {
                var stores = await GetByIdsAsync(ids);
                var dbStores = await repository.GetStoresByIdsAsync(ids);

                foreach (var store in stores)
                {
                    _commerceService.DeleteSeoForObject(store);
                    await _dynamicPropertyService.DeleteDynamicPropertyValuesAsync(store);
                    //Deep remove settings
                    await _settingManager.RemoveEntitySettingsAsync(store);

                    var dbStore = dbStores.FirstOrDefault(x => x.Id == store.Id);
                    if (dbStore != null)
                    {
                        repository.Remove(dbStore);
                    }
                }
                await repository.UnitOfWork.CommitAsync(); 
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
                var store = await GetByIdAsync(user.StoreId);
                if (store != null)
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

        private void ValidateStoreProperties(Store store)
        {
            if (store == null)
            {
                throw new ArgumentNullException(nameof(store));
            }

            var validator = new StoreValidator();
            validator.ValidateAndThrow(store);
        }

        #endregion
    }
}
