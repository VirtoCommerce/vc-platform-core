using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using VirtoCommerce.CoreModule.Core.Seo;
using VirtoCommerce.CoreModule.Data.Caching;
using VirtoCommerce.CoreModule.Data.Repositories;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Events;

namespace VirtoCommerce.CoreModule.Data.Seo
{
    public class SeoService : ISeoService
    {
        private readonly Func<ICoreRepository> _repositoryFactory;
        private readonly IEventPublisher _eventPublisher;
        private readonly IPlatformMemoryCache _platformMemoryCache;

        public SeoService(Func<ICoreRepository> repositoryFactory, IEventPublisher eventPublisher, IPlatformMemoryCache platformMemoryCache)
        {
            _repositoryFactory = repositoryFactory;
            _eventPublisher = eventPublisher;
            _platformMemoryCache = platformMemoryCache;
        }

        public async Task LoadSeoForObjectsAsync(ISeoSupport[] seoSupportObjects)
        {
            var cacheKey = CacheKey.With(GetType(), "LoadSeoForObjectsAsync", string.Join("-", seoSupportObjects.Select(x => $"{x.GetType()} : { x.Id }").Distinct()));
            await _platformMemoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                using (var repository = _repositoryFactory())
                {
                    var objectIds = seoSupportObjects.Where(x => x.Id != null).Select(x => x.Id).Distinct().ToArray();

                    var seoInfosEntities = await repository.SeoUrlKeywords.Where(x => objectIds.Contains(x.ObjectId)).ToArrayAsync();
                    var seoInfos = seoInfosEntities.Select(x => x.ToModel(AbstractTypeFactory<SeoInfo>.TryCreateInstance())).ToList();

                    foreach (var seoSupportObject in seoSupportObjects)
                    {
                        seoSupportObject.SeoInfos = seoInfos.Where(x => x.ObjectId == seoSupportObject.Id && x.ObjectType == seoSupportObject.SeoObjectType).ToList();
                        foreach (var seoInfo in seoSupportObject.SeoInfos)
                        {
                            cacheEntry.AddExpirationToken(SeoInfoCacheRegion.CreateChangeToken(seoInfo.Id));
                        }
                    }
                }
                return Task.CompletedTask;
            });
        }

        public async Task SaveSeoForObjectsAsync(ISeoSupport[] seoSupportObjects)
        {
            if (seoSupportObjects == null)
            {
                throw new ArgumentNullException(nameof(seoSupportObjects));
            }

            var changedEntries = new List<GenericChangedEntry<SeoInfo>>();
            var pkMap = new PrimaryKeyResolvingMap();
            foreach (var seoObject in seoSupportObjects.Where(x => x.Id != null))
            {
                var objectType = seoObject.SeoObjectType;

                using (var repository = _repositoryFactory())
                {
                    if (seoObject.SeoInfos != null)
                    {
                        // Normalize seoInfo
                        foreach (var seoInfo in seoObject.SeoInfos)
                        {
                            if (seoInfo.ObjectId == null)
                                seoInfo.ObjectId = seoObject.Id;

                            if (seoInfo.ObjectType == null)
                                seoInfo.ObjectType = objectType;
                        }
                    }

                    if (seoObject.SeoInfos != null)
                    {
                        var target = new List<SeoUrlKeywordEntity>(await repository.GetObjectSeoUrlKeywordsAsync(objectType, seoObject.Id));
                        var source = new List<SeoUrlKeywordEntity>(seoObject.SeoInfos.Select(x => AbstractTypeFactory<SeoUrlKeywordEntity>.TryCreateInstance().FromModel(x, pkMap)));

                        var seoComparer = AnonymousComparer.Create((SeoUrlKeywordEntity x) => x.Id ?? string.Join(":", x.StoreId, x.ObjectId, x.ObjectType, x.Language));
                        source.CompareTo(target, seoComparer, (state, sourceEntity, targetEntity) =>
                        {
                            if (state == EntryState.Added)
                            {
                                repository.Add(sourceEntity);
                            }
                            if (state == EntryState.Deleted)
                            {
                                repository.Remove(sourceEntity);
                            }
                            if (state == EntryState.Modified)
                            {
                                sourceEntity.Patch(targetEntity);
                            }
                        });
                        await repository.UnitOfWork.CommitAsync();
                        pkMap.ResolvePrimaryKeys();
                        SeoInfoCacheRegion.ExpireSeoInfos(source.Concat(target).Distinct().Select(x => x.Id));
                    }
                }
            }
        }

        public async Task DeleteSeoForObjectAsync(ISeoSupport seoSupportObject)
        {
            if (seoSupportObject == null)
            {
                throw new ArgumentNullException(nameof(seoSupportObject));
            }

            if (seoSupportObject.Id != null)
            {
                var changedEntries = new List<GenericChangedEntry<SeoInfo>>();

                using (var repository = _repositoryFactory())
                {
                    var objectType = seoSupportObject.SeoObjectType;
                    var objectId = seoSupportObject.Id;
                    var seoUrlKeywords = await repository.GetObjectSeoUrlKeywordsAsync(objectType, objectId);

                    foreach (var seoUrlKeyword in seoUrlKeywords)
                    {
                        repository.Remove(seoUrlKeyword);
                    }
                    await repository.UnitOfWork.CommitAsync();
                    SeoInfoCacheRegion.ExpireSeoInfos(seoUrlKeywords.Select(x => x.Id));
                }
            }
        }

        public async Task<IEnumerable<SeoInfo>> GetAllSeoDuplicatesAsync()
        {
            var cacheKey = CacheKey.With(GetType(), "GetAllSeoDuplicatesAsync");
            return await _platformMemoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                var result = new List<SeoInfo>();
                using (var repository = _repositoryFactory())
                {
                    var dublicateSeoRecords = await repository.SeoUrlKeywords.GroupBy(x => x.Keyword + ":" + x.StoreId).Where(x => x.Count() > 1)
                                                              .SelectMany(x => x).ToArrayAsync();
                    result = dublicateSeoRecords.Select(x => x.ToModel(AbstractTypeFactory<SeoInfo>.TryCreateInstance())).ToList();
                    foreach (var duplicateSeoEntity in dublicateSeoRecords)
                    {
                        var seoInfo = duplicateSeoEntity.ToModel(AbstractTypeFactory<SeoInfo>.TryCreateInstance());
                        cacheEntry.AddExpirationToken(SeoInfoCacheRegion.CreateChangeToken(seoInfo.Id));
                    }
                }
                return result;
            });
        }

        public async Task<IEnumerable<SeoInfo>> GetSeoByKeywordAsync(string keyword)
        {
            var cacheKey = CacheKey.With(GetType(), "GetSeoByKeywordAsync", keyword);
            return await _platformMemoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                using (var repository = _repositoryFactory())
                {
                    // Find seo entries for specified keyword. Also add other seo entries related to found object.
                    var query = await repository.SeoUrlKeywords.Where(x => x.Keyword == keyword)
                                                               .Join(repository.SeoUrlKeywords, x => new { x.ObjectId, x.ObjectType }, y => new { y.ObjectId, y.ObjectType }, (x, y) => y)
                                                               .ToArrayAsync();
                    var result = query.Select(x =>
                    {
                        var seoInfo = x.ToModel(AbstractTypeFactory<SeoInfo>.TryCreateInstance());
                        cacheEntry.AddExpirationToken(SeoInfoCacheRegion.CreateChangeToken(seoInfo.Id));
                        return seoInfo;
                    }).ToList();
                    return result;
                }
            });
        }

        public async Task SaveSeoInfosAsync(SeoInfo[] seoinfos)
        {
            var pkMap = new PrimaryKeyResolvingMap();
            using (var repository = _repositoryFactory())
            {
                var target = new List<SeoUrlKeywordEntity>(await repository.GetSeoByIdsAsync(seoinfos.Select(x => x.Id).ToArray()));
                var source = new List<SeoUrlKeywordEntity>(seoinfos.Select(x => AbstractTypeFactory<SeoUrlKeywordEntity>.TryCreateInstance().FromModel(x, pkMap)));

                source.CompareTo(target, EqualityComparer<SeoUrlKeywordEntity>.Default, (state, sourceEntity, targetEntity) =>
               {
                   if (state == EntryState.Added)
                   {
                       repository.Add(sourceEntity);
                   }
                   if (state == EntryState.Modified)
                   {
                       sourceEntity.Patch(targetEntity);
                   }
               });

                await repository.UnitOfWork.CommitAsync();
                pkMap.ResolvePrimaryKeys();
                SeoInfoCacheRegion.ExpireSeoInfos(source.Select(x => x.Id));
            }
        }
    }
}
