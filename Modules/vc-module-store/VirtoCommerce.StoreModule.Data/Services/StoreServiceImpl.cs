using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using VirtoCommerce.Domain.Commerce.Model;
using VirtoCommerce.Domain.Commerce.Services;
using VirtoCommerce.Domain.Payment.Services;
using VirtoCommerce.Domain.Shipping.Services;
using VirtoCommerce.Domain.Store.Model;
using VirtoCommerce.Domain.Store.Services;
using VirtoCommerce.Domain.Tax.Services;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.DynamicProperties;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.Platform.Data.Infrastructure;
using VirtoCommerce.StoreModule.Data.Model;
using VirtoCommerce.StoreModule.Data.Repositories;
using VirtoCommerce.StoreModule.Data.Services.Validation;

namespace VirtoCommerce.StoreModule.Data.Services
{
    public class StoreServiceImpl : ServiceBase, IStoreService
    {
        private readonly Func<IStoreRepository> _repositoryFactory;
        private readonly ICommerceService _commerceService;
        private readonly ISettingsManager _settingManager;
        private readonly IDynamicPropertyService _dynamicPropertyService;
        private readonly IShippingMethodsService _shippingService;
        private readonly IPaymentMethodsService _paymentService;
        private readonly ITaxService _taxService;

        public StoreServiceImpl(Func<IStoreRepository> repositoryFactory, ICommerceService commerceService, ISettingsManager settingManager,
                                IDynamicPropertyService dynamicPropertyService, IShippingMethodsService shippingService, IPaymentMethodsService paymentService,
                                ITaxService taxService)
        {
            _repositoryFactory = repositoryFactory;
            _commerceService = commerceService;
            _settingManager = settingManager;
            _dynamicPropertyService = dynamicPropertyService;
            _shippingService = shippingService;
            _paymentService = paymentService;
            _taxService = taxService;
        }

        #region IStoreService Members

        public Store[] GetByIds(string[] ids)
        {
            var stores = new List<Store>();

            var fulfillmentCenters = _commerceService.GetAllFulfillmentCenters().ToList();
            using (var repository = _repositoryFactory())
            {               
                var dbStores = repository.GetStoresByIds(ids);
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
                    store.Settings = _settingManager.GetModuleSettings("VirtoCommerce.Store");
                    _settingManager.LoadEntitySettingsValues(store);
                    stores.Add(store);
                }
            }

            var result = stores.ToArray();
            _dynamicPropertyService.LoadDynamicPropertyValues(result);
            _commerceService.LoadSeoForObjects(result);
            return result;
        }

        public Store GetById(string id)
        {
            return GetByIds(new[] { id }).FirstOrDefault();
        }

        public Store Create(Store store)
        {
            var pkMap = new PrimaryKeyResolvingMap();

            ValidateStoreProperties(store);

            var dbStore = AbstractTypeFactory<StoreEntity>.TryCreateInstance();
            dbStore = dbStore.FromModel(store, pkMap);

            using (var repository = _repositoryFactory())
            {
                repository.Add(dbStore);
                CommitChanges(repository);
                pkMap.ResolvePrimaryKeys();
            }

            //Need add seo separately
            _commerceService.UpsertSeoForObjects(new[] { store });

            //Deep save properties
            _dynamicPropertyService.SaveDynamicPropertyValues(store);
            //Deep save settings
            _settingManager.SaveEntitySettingsValues(store);

            var retVal = GetById(store.Id);
            return retVal;
        }

        public void Update(Store[] stores)
        {
            var pkMap = new PrimaryKeyResolvingMap();

            using (var repository = _repositoryFactory())
            using (var changeTracker = base.GetChangeTracker(repository))
            {
                var dbStores = repository.GetStoresByIds(stores.Select(x => x.Id).ToArray());
                foreach (var store in stores)
                {
                    var sourceEntity = AbstractTypeFactory<StoreEntity>.TryCreateInstance().FromModel(store, pkMap);
                    var targetEntity = dbStores.First(x => x.Id == store.Id);

                    if (targetEntity != null)
                    {
                        changeTracker.Attach(targetEntity);
                        sourceEntity.Patch(targetEntity);

                        _dynamicPropertyService.SaveDynamicPropertyValues(store);
                        //Deep save settings
                        _settingManager.SaveEntitySettingsValues(store);

                        //Patch SeoInfo  separately
                        _commerceService.UpsertSeoForObjects(stores);
                    }
                }

                CommitChanges(repository);
            }
        }

        public void Delete(string[] ids)
        {
            using (var repository = _repositoryFactory())
            {
                var stores = GetByIds(ids);
                var dbStores = repository.GetStoresByIds(ids);

                foreach (var store in stores)
                {
                    _commerceService.DeleteSeoForObject(store);
                    _dynamicPropertyService.DeleteDynamicPropertyValues(store);
                    //Deep remove settings
                    _settingManager.RemoveEntitySettings(store);

                    var dbStore = dbStores.FirstOrDefault(x => x.Id == store.Id);
                    if (dbStore != null)
                    {
                        repository.Remove(dbStore);
                    }
                }
                CommitChanges(repository);
            }
        }

        public SearchResult SearchStores(SearchCriteria criteria)
        {
            var retVal = new SearchResult();
            using (var repository = _repositoryFactory())
            {
                var query = repository.Stores;
                if (!string.IsNullOrEmpty(criteria.Keyword))
                {
                    query = query.Where(x => x.Name.Contains(criteria.Keyword) || x.Id.Contains(criteria.Keyword));
                }
                if (!criteria.StoreIds.IsNullOrEmpty())
                {
                    query = query.Where(x => criteria.StoreIds.Contains(x.Id));
                }
                var sortInfos = criteria.SortInfos;
                if (sortInfos.IsNullOrEmpty())
                {
                    sortInfos = new[] { new SortInfo { SortColumn = "Name" } };
                }

                query = query.OrderBySortInfos(sortInfos);

                retVal.TotalCount = query.Count();
                var storeIds = query.Skip(criteria.Skip)
                                 .Take(criteria.Take)
                                 .Select(x => x.Id)
                                 .ToArray();

                retVal.Stores = GetByIds(storeIds).AsQueryable().OrderBySortInfos(sortInfos).ToList();
            }
            return retVal;
        }


        /// <summary>
        /// Returns list of stores ids which passed user can signIn
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public IEnumerable<string> GetUserAllowedStoreIds(ApplicationUserExtended user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            var retVal = new List<string>();

            if (user.StoreId != null)
            {
                var store = GetById(user.StoreId);
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
