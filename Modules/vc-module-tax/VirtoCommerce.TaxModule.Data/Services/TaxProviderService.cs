using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.Platform.Data.Infrastructure;
using VirtoCommerce.TaxModule.Core.Model;
using VirtoCommerce.TaxModule.Core.Services;
using VirtoCommerce.TaxModule.Data.Caching;
using VirtoCommerce.TaxModule.Data.Model;
using VirtoCommerce.TaxModule.Data.Repositories;

namespace VirtoCommerce.TaxModule.Data.Services
{
    public class TaxProviderService : ITaxProviderService, ITaxProviderRegistrar
    {
        private readonly Func<ITaxRepository> _repositoryFactory;
        private readonly ISettingsManager _settingManager;
        private readonly IPlatformMemoryCache _memCache;

        public TaxProviderService(Func<ITaxRepository> repositoryFactory, ISettingsManager settingManager, IPlatformMemoryCache platformMemoryCache)
        {
            _repositoryFactory = repositoryFactory;
            _settingManager = settingManager;
            _memCache = platformMemoryCache;
        }

        #region ITaxProviderRegistrar members
        public void RegisterTaxProvider<T>(Func<T> factory = null) where T : TaxProvider
        {
            if (AbstractTypeFactory<TaxProvider>.AllTypeInfos.All(t => t.Type != typeof(T)))
            {
                var typeInfo = AbstractTypeFactory<TaxProvider>.RegisterType<T>();
                if (factory != null)
                {
                    typeInfo.WithFactory(factory);
                }
            }
        }
        #endregion

        public virtual async Task<TaxProvider[]> GetByIdsAsync(string[] ids, string responseGroup)
        {
            var cacheKey = CacheKey.With(GetType(), "GetByIdsAsync", string.Join("-", ids));
            return await _memCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                var result = new List<TaxProvider>();

                using (var repository = _repositoryFactory())
                {
                    repository.DisableChangesTracking();
                    var existEntities = await repository.GetStoreTaxProviderByIdsAsync(ids, responseGroup);
                    foreach (var existEntity in existEntities)
                    {
                        var taxProvider = AbstractTypeFactory<TaxProvider>.TryCreateInstance(string.IsNullOrEmpty(existEntity.TypeName) ? $"{existEntity.Code}TaxProvider" : existEntity.TypeName);
                        if (taxProvider != null)
                        {
                            existEntity.ToModel(taxProvider);

                            await _settingManager.DeepLoadSettingsAsync(taxProvider);
                            result.Add(taxProvider);

                        }
                    }
                    cacheEntry.AddExpirationToken(TaxCacheRegion.CreateChangeToken());
                    return result.ToArray();
                }
            });
        }

        public virtual async Task<TaxProvider> GetByIdAsync(string id, string responseGroup)
        {
            return (await GetByIdsAsync(new[] { id }, responseGroup)).FirstOrDefault();
        }

        public virtual async Task SaveChangesAsync(TaxProvider[] taxProviders)
        {
            var pkMap = new PrimaryKeyResolvingMap();

            using (var repository = _repositoryFactory())
            {
                var dataExistEntities = await repository.GetStoreTaxProviderByIdsAsync(taxProviders.Where(x => !x.IsTransient()).Select(x => x.Id).ToArray());
                foreach (var taxProvider in taxProviders)
                {
                    var originalEntity = dataExistEntities.FirstOrDefault(x => x.Id == taxProvider.Id);
                    var modifiedEntity = AbstractTypeFactory<StoreTaxProviderEntity>.TryCreateInstance().FromModel(taxProvider, pkMap);
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
                await _settingManager.DeepSaveSettingsAsync(taxProviders);
            }

            TaxCacheRegion.ExpireRegion();
        }

        public virtual async Task DeleteAsync(string[] ids)
        {
            var taxProviders = await GetByIdsAsync(ids, null);
            using (var repository = _repositoryFactory())
            {
                var taxProviderEntities = await repository.GetStoreTaxProviderByIdsAsync(ids);
                foreach (var taxProviderEntity in taxProviderEntities)
                {
                    repository.Remove(taxProviderEntity);
                }
                await repository.UnitOfWork.CommitAsync();
            }

            await _settingManager.DeepRemoveSettingsAsync(taxProviders);

        }
    }
}
