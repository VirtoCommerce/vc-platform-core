using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using VirtoCommerce.PaymentModule.Core.Model;
using VirtoCommerce.PaymentModule.Core.Services;
using VirtoCommerce.PaymentModule.Data.Caching;
using VirtoCommerce.PaymentModule.Data.Model;
using VirtoCommerce.PaymentModule.Data.Repositories;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.Platform.Data.Infrastructure;

namespace VirtoCommerce.PaymentModule.Data.Services
{
    public class PaymentMethodsService : IPaymentMethodsService, IPaymentMethodsRegistrar
    {
        private readonly Func<IPaymentRepository> _repositoryFactory;
        private readonly ISettingsManager _settingManager;
        private readonly IPlatformMemoryCache _memCache;

        public PaymentMethodsService(
            Func<IPaymentRepository> repositoryFactory,
            ISettingsManager settingManager,
            IPlatformMemoryCache memCache)
        {
            _repositoryFactory = repositoryFactory;
            _settingManager = settingManager;
            _memCache = memCache;
        }

        #region IPaymentMethodsRegistrar members
        public void RegisterPaymentMethod<T>(Func<T> factory = null) where T : PaymentMethod
        {
            if (AbstractTypeFactory<PaymentMethod>.AllTypeInfos.All(t => t.Type != typeof(T)))
            {
                var typeInfo = AbstractTypeFactory<PaymentMethod>.RegisterType<T>();
                if (factory != null)
                {
                    typeInfo.WithFactory(factory);
                }
            }
        }
        #endregion

        public async Task<PaymentMethod[]> GetByIdsAsync(string[] ids, string responseGroup)
        {
            var cacheKey = CacheKey.With(GetType(), "GetByIdsAsync", string.Join("-", ids));
            return await _memCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                var result = new List<PaymentMethod>();

                using (var repository = _repositoryFactory())
                {
                    repository.DisableChangesTracking();
                    var existEntities = await repository.GetStorePaymentMethodsByIdsAsync(ids, responseGroup);
                    foreach (var existEntity in existEntities)
                    {
                        var paymentMethod = AbstractTypeFactory<PaymentMethod>.TryCreateInstance(string.IsNullOrEmpty(existEntity.TypeName) ? existEntity.Code : existEntity.TypeName);
                        if (paymentMethod != null)
                        {
                            existEntity.ToModel(paymentMethod);

                            await _settingManager.DeepLoadSettingsAsync(paymentMethod);
                            result.Add(paymentMethod);
                        }
                    }
                    cacheEntry.AddExpirationToken(PaymentCacheRegion.CreateChangeToken());
                    return result.ToArray();
                }
            });
        }

        public async Task<PaymentMethod> GetByIdAsync(string id, string responseGroup)
        {
            return (await GetByIdsAsync(new[] { id }, responseGroup)).FirstOrDefault();
        }

        public async Task SaveChangesAsync(PaymentMethod[] paymentMethods)
        {
            var pkMap = new PrimaryKeyResolvingMap();

            using (var repository = _repositoryFactory())
            {
                var dataExistEntities = await repository.GetStorePaymentMethodsByIdsAsync(paymentMethods.Where(x => !x.IsTransient())
                    .Select(x => x.Id)
                    .ToArray());

                foreach (var paymentMethod in paymentMethods)
                {
                    var originalEntity = dataExistEntities.FirstOrDefault(x => x.Id == paymentMethod.Id);
                    var modifiedEntity = AbstractTypeFactory<StorePaymentMethodEntity>.TryCreateInstance().FromModel(paymentMethod, pkMap);
                    if (originalEntity != null)
                    {
                        modifiedEntity.Patch(originalEntity);
                    }
                    else
                    {
                        repository.Add(modifiedEntity);
                    }
                }

                //Raise domain events
                await repository.UnitOfWork.CommitAsync();
                pkMap.ResolvePrimaryKeys();
                //Save settings
                await _settingManager.DeepSaveSettingsAsync(paymentMethods);
            }

            PaymentCacheRegion.ExpireRegion();
        }

        public async Task DeleteAsync(string[] ids)
        {
            var paymentMethods = await GetByIdsAsync(ids, null);
            using (var repository = _repositoryFactory())
            {
                var paymentMethodEntities = await repository.GetStorePaymentMethodsByIdsAsync(ids);
                foreach (var paymentMethodEntity in paymentMethodEntities)
                {
                    repository.Remove(paymentMethodEntity);
                }
                await repository.UnitOfWork.CommitAsync();
            }

            await _settingManager.DeepRemoveSettingsAsync(paymentMethods);
        }
    }
}
