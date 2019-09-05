using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.CatalogModule.Core.Services;
using VirtoCommerce.CatalogModule.Data.Caching;
using VirtoCommerce.CatalogModule.Data.Model;
using VirtoCommerce.CatalogModule.Data.Repositories;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Data.Infrastructure;

namespace VirtoCommerce.CatalogModule.Data.Services
{
    public class PropertyDictionaryItemService : IPropertyDictionaryItemService
    {
        private readonly Func<ICatalogRepository> _repositoryFactory;
        private readonly IPlatformMemoryCache _platformMemoryCache;

        public PropertyDictionaryItemService(Func<ICatalogRepository> repositoryFactory, IPlatformMemoryCache platformMemoryCache)
        {
            _repositoryFactory = repositoryFactory;
            _platformMemoryCache = platformMemoryCache;
        }

        public Task<PropertyDictionaryItem[]> GetByIdsAsync(string[] ids)
        {
            var cacheKey = CacheKey.With(GetType(), "GetByIdsAsync", string.Join(",", ids));
            return _platformMemoryCache.GetOrCreateExclusive(cacheKey, async (cacheEntry) =>
            {
                cacheEntry.AddExpirationToken(CatalogCacheRegion.CreateChangeToken());
                cacheEntry.AddExpirationToken(DictionaryItemsCacheRegion.CreateChangeToken());
                PropertyDictionaryItem[] result;

                using (var repository = _repositoryFactory())
                {
                    //Optimize performance and CPU usage
                    repository.DisableChangesTracking();

                    result = (await repository.GetPropertyDictionaryItemsByIdsAsync(ids))
                        .Select(x => x.ToModel(AbstractTypeFactory<PropertyDictionaryItem>.TryCreateInstance()))
                        .ToArray();
                }
                return result;
            });
        }

        public async Task SaveChangesAsync(PropertyDictionaryItem[] dictItems)
        {
            if (dictItems == null)
            {
                throw new ArgumentNullException(nameof(dictItems));
            }

            var pkMap = new PrimaryKeyResolvingMap();
            using (var repository = _repositoryFactory())
            {
                var dbExistEntities = await repository.GetPropertyDictionaryItemsByIdsAsync(dictItems.Where(x => !x.IsTransient()).Select(x => x.Id).ToArray());
                foreach (var dictItem in dictItems)
                {
                    var originalEntity = dbExistEntities.FirstOrDefault(x => x.Id == dictItem.Id);
                    var modifiedEntity = AbstractTypeFactory<PropertyDictionaryItemEntity>.TryCreateInstance().FromModel(dictItem, pkMap);
                    if (originalEntity != null)
                    {
                        modifiedEntity.Patch(originalEntity);
                    }
                    else
                    {
                        repository.Add(modifiedEntity);
                    }
                }

                await repository.UnitOfWork.CommitAsync();
                pkMap.ResolvePrimaryKeys();
            }

            DictionaryItemsCacheRegion.ExpireRegion();
        }

        public async Task DeleteAsync(string[] ids)
        {
            using (var repository = _repositoryFactory())
            {
                var dbEntities = await repository.GetPropertyDictionaryItemsByIdsAsync(ids);

                foreach (var dbEntity in dbEntities)
                {
                    repository.Remove(dbEntity);
                }
                await repository.UnitOfWork.CommitAsync();
            }

            DictionaryItemsCacheRegion.ExpireRegion();
        }
    }
}

