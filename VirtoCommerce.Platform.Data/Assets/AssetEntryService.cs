using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Caching.Memory;
using VirtoCommerce.Platform.Core.Assets;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Data.Caching;
using VirtoCommerce.Platform.Data.Repositories;

namespace VirtoCommerce.Platform.Data.Assets
{
    public class AssetEntryService:  IAssetEntryService, IAssetEntrySearchService
    {
        private readonly Func<IPlatformRepository> _platformRepository;
        private readonly IBlobUrlResolver _blobUrlResolver;
        private readonly IMemoryCache _memoryCache;

        public AssetEntryService(Func<IPlatformRepository> repositoryFactory, IBlobUrlResolver blobUrlResolver, IMemoryCache memoryCache)
        {
            _platformRepository = repositoryFactory;
            _blobUrlResolver = blobUrlResolver;
            _memoryCache = memoryCache;
        }

        public  GenericSearchResult<AssetEntry> SearchAssetEntries(AssetEntrySearchCriteria criteria)
        {
            var cacheKey = CacheKey.With(GetType(), "SearchAssetEntries", criteria.GetHashCode().ToString());

            return _memoryCache.GetOrCreateExclusive(cacheKey, (cacheEntry) =>
            {
                criteria = criteria ?? new AssetEntrySearchCriteria();

                using (var repository = _platformRepository())
                {

                    cacheEntry.AddExpirationToken(AssetCacheRegion.CreateChangeToken());

                    var query = repository.AssetEntries;

                    if (!string.IsNullOrEmpty(criteria.SearchPhrase))
                    {
                        query = query.Where(x => x.Name.Contains(criteria.SearchPhrase) || x.RelativeUrl.Contains(criteria.SearchPhrase));
                    }

                    if (!string.IsNullOrEmpty(criteria.LanguageCode))
                    {
                        query = query.Where(x => x.LanguageCode == criteria.LanguageCode);
                    }

                    if (!string.IsNullOrEmpty(criteria.Group))
                    {
                        query = query.Where(x => x.Group == criteria.Group);
                    }

                    if (!criteria.Tenants.IsNullOrEmpty())
                    {
                        var tenants = criteria.Tenants.Where(x => x.IsValid).ToArray();
                        if (tenants.Any())
                        {
                            var tenantsStrings = tenants.Select(x => x.ToString());
                            query = query.Where(x => tenantsStrings.Contains(x.TenantId + "_" + x.TenantType));
                        }
                    }

                    var result = new GenericSearchResult<AssetEntry>()
                    {
                        TotalCount = query.Count()
                    };

                    var sortInfos = criteria.SortInfos;
                    if (sortInfos.IsNullOrEmpty())
                    {
                        sortInfos = new[] { new SortInfo { SortColumn = "CreatedDate", SortDirection = SortDirection.Descending } };
                    }
                    query = query.OrderBySortInfos(sortInfos);

                    var ids = query
                        .Skip(criteria.Skip)
                        .Take(criteria.Take)
                        .Select(x => x.Id).ToList();

                    result.Results = repository.GetAssetsByIds(ids)
                        .Select(x => x.ToModel(AbstractTypeFactory<AssetEntry>.TryCreateInstance(), _blobUrlResolver))
                        .OrderBy(x => ids.IndexOf(x.Id))
                        .ToList();

                    return result;
                }
            });
        }

        public IEnumerable<AssetEntry> GetByIds(IEnumerable<string> ids)
        {
            using (var repository = _platformRepository())
            {
                var entities = repository.GetAssetsByIds(ids);
                return entities.Select(x => x.ToModel(AbstractTypeFactory<AssetEntry>.TryCreateInstance(), _blobUrlResolver));
            }
        }

        public void SaveChanges(IEnumerable<AssetEntry> entries)
        {
            using (var repository = _platformRepository())
            {
                var nonTransientEntryIds = entries.Where(x => !x.IsTransient()).Select(x => x.Id).ToArray();
                var originalDataEntities = repository.AssetEntries.Where(x => nonTransientEntryIds.Contains(x.Id)).ToList();
                foreach (var entry in entries)
                {
                    var originalEntity = originalDataEntities.FirstOrDefault(x => x.Id == entry.Id);
                    var modifiedEntity = AbstractTypeFactory<AssetEntryEntity>.TryCreateInstance().FromModel(entry);
                    if (originalEntity != null)
                    {
                        modifiedEntity.Patch(originalEntity);
                    }
                    else
                    {
                        repository.Add(modifiedEntity);
                    }
                }

                repository.UnitOfWork.CommitAsync();

                //Reset cached items
                AssetCacheRegion.ExpireRegion();
            }
        }

        public void Delete(IEnumerable<string> ids)
        {
            if (ids == null)
                throw new ArgumentNullException(nameof(ids));

            using (var repository = _platformRepository())
            {
                var items = repository.AssetEntries
                    .Where(p => ids.Contains(p.Id))
                    .ToList();

                foreach (var item in items)
                {
                    repository.Remove(item);
                }

                repository.UnitOfWork.Commit();

                AssetCacheRegion.ExpireRegion();
            }
        }
    }
}
