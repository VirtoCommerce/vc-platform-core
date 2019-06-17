using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.Platform.Data.Infrastructure;
using VirtoCommerce.ShippingModule.Core.Model;
using VirtoCommerce.ShippingModule.Core.Services;
using VirtoCommerce.ShippingModule.Data.Caching;
using VirtoCommerce.ShippingModule.Data.Model;
using VirtoCommerce.ShippingModule.Data.Repositories;

namespace VirtoCommerce.ShippingModule.Data.Services
{
    public class ShippingMethodsService : IShippingMethodsRegistrar, IShippingMethodsService
    {
        private readonly Func<IShippingRepository> _repositoryFactory;
        private readonly ISettingsManager _settingManager;
        private readonly IPlatformMemoryCache _memCache;

        public ShippingMethodsService(
            Func<IShippingRepository> repositoryFactory,
            ISettingsManager settingManager,
            IPlatformMemoryCache memCache
            )
        {
            _repositoryFactory = repositoryFactory;
            _settingManager = settingManager;
            _memCache = memCache;
        }

        #region IShippingMethodsRegistrar members

        public void RegisterShippingMethod<T>(Func<T> factory = null) where T : ShippingMethod
        {
            if (AbstractTypeFactory<ShippingMethod>.AllTypeInfos.All(t => t.Type != typeof(T)))
            {
                var typeInfo = AbstractTypeFactory<ShippingMethod>.RegisterType<T>();
                if (factory != null)
                {
                    typeInfo.WithFactory(factory);
                }
            }
        }
        #endregion

        public async Task<ShippingMethod[]> GetByIdsAsync(string[] ids, string responseGroup)
        {
            var cacheKey = CacheKey.With(GetType(), "GetByIdsAsync", string.Join("-", ids));
            return await _memCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                var result = new List<ShippingMethod>();

                using (var repository = _repositoryFactory())
                {
                    repository.DisableChangesTracking();
                    var existEntities = await repository.GetStoreShippingMethodsByIdsAsync(ids, responseGroup);
                    foreach (var existEntity in existEntities)
                    {
                        var shippingMethod = AbstractTypeFactory<ShippingMethod>.TryCreateInstance(string.IsNullOrEmpty(existEntity.TypeName) ? $"{existEntity.Code}ShippingMethod" : existEntity.TypeName);
                        if (shippingMethod != null)
                        {
                            existEntity.ToModel(shippingMethod);

                            await _settingManager.DeepLoadSettingsAsync(shippingMethod);
                            result.Add(shippingMethod);
                        }
                    }
                    cacheEntry.AddExpirationToken(ShippingCacheRegion.CreateChangeToken());
                    return result.ToArray();
                }
            });
        }

        public async Task<ShippingMethod> GetByIdAsync(string id, string responseGroup)
        {
            return (await GetByIdsAsync(new[] { id }, responseGroup)).FirstOrDefault();
        }

        public async Task SaveChangesAsync(ShippingMethod[] shippingMethods)
        {
            var pkMap = new PrimaryKeyResolvingMap();

            using (var repository = _repositoryFactory())
            {
                var dataExistEntities = await repository.GetStoreShippingMethodsByIdsAsync(shippingMethods.Where(x => !x.IsTransient())
                    .Select(x => x.Id)
                    .ToArray());

                foreach (var shippingMethod in shippingMethods)
                {
                    var originalEntity = dataExistEntities.FirstOrDefault(x => x.Id == shippingMethod.Id);
                    var modifiedEntity = AbstractTypeFactory<StoreShippingMethodEntity>.TryCreateInstance().FromModel(shippingMethod, pkMap);
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
                await _settingManager.DeepSaveSettingsAsync(shippingMethods);
            }

            ShippingCacheRegion.ExpireRegion();
        }

        public async Task DeleteAsync(string[] ids)
        {
            var shippingMethods = await GetByIdsAsync(ids, null);
            using (var repository = _repositoryFactory())
            {
                var shippingMethodEntities = await repository.GetStoreShippingMethodsByIdsAsync(ids);
                foreach (var shippingMethodEntity in shippingMethodEntities)
                {
                    repository.Remove(shippingMethodEntity);
                }
                await repository.UnitOfWork.CommitAsync();
            }

            await _settingManager.DeepRemoveSettingsAsync(shippingMethods);
        }
    }
}
