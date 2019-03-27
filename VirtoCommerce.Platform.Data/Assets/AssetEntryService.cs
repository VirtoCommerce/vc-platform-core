using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VirtoCommerce.Platform.Core.Assets;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Data.Model;
using VirtoCommerce.Platform.Data.Repositories;

namespace VirtoCommerce.Platform.Data.Assets
{
    public class AssetEntryService : IAssetEntryService, IAssetEntrySearchService
    {
        private readonly Func<IPlatformRepository> _platformRepository;
        private readonly IBlobUrlResolver _blobUrlResolver;

        public AssetEntryService(Func<IPlatformRepository> repositoryFactory, IBlobUrlResolver blobUrlResolver)
        {
            _platformRepository = repositoryFactory;
            _blobUrlResolver = blobUrlResolver;
        }

        public async Task<GenericSearchResult<AssetEntry>> SearchAssetEntriesAsync(AssetEntrySearchCriteria criteria)
        {
            criteria = criteria ?? AbstractTypeFactory<AssetEntrySearchCriteria>.TryCreateInstance();

            using (var repository = _platformRepository())
            {
                var sortInfos = GetSearchAssetEntriesSortInfo(criteria);
                var query = GetSearchAssetEntriesQuery(criteria, repository, sortInfos);

                var result = new GenericSearchResult<AssetEntry>
                {
                    TotalCount = await query.CountAsync()
                };

                var ids = await query
                    .Skip(criteria.Skip)
                    .Take(criteria.Take)
                    .Select(x => x.Id)
                    .ToListAsync();

                var asetsResult = await repository.GetAssetsByIdsAsync(ids);

                result.Results = asetsResult
                    .Select(x => x.ToModel(AbstractTypeFactory<AssetEntry>.TryCreateInstance(), _blobUrlResolver))
                    .OrderBy(x => ids.IndexOf(x.Id))
                    .ToList();

                return result;
            }
        }

        public async Task<IEnumerable<AssetEntry>> GetByIdsAsync(IEnumerable<string> ids)
        {
            using (var repository = _platformRepository())
            {
                var entities = await repository.GetAssetsByIdsAsync(ids);
                return entities.Select(x => x.ToModel(AbstractTypeFactory<AssetEntry>.TryCreateInstance(), _blobUrlResolver));
            }
        }

        public async Task SaveChangesAsync(IEnumerable<AssetEntry> entries)
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

                await repository.UnitOfWork.CommitAsync();

                //Reset cached items
                AssetCacheRegion.ExpireRegion();
            }
        }

        public async Task DeleteAsync(IEnumerable<string> ids)
        {
            if (ids == null)
                throw new ArgumentNullException(nameof(ids));

            using (var repository = _platformRepository())
            {
                var items = await repository.AssetEntries
                    .Where(p => ids.Contains(p.Id))
                    .ToListAsync();

                foreach (var item in items)
                {
                    repository.Remove(item);
                }

                await repository.UnitOfWork.CommitAsync();

                AssetCacheRegion.ExpireRegion();
            }
        }

        private IList<SortInfo> GetSearchAssetEntriesSortInfo(AssetEntrySearchCriteria criteria)
        {
            var sortInfos = criteria.SortInfos;
            if (sortInfos.IsNullOrEmpty())
            {
                sortInfos = new[]
                {
                    new SortInfo
                    {
                        SortColumn = "CreatedDate",
                        SortDirection = SortDirection.Descending
                    }
                };
            }

            return sortInfos;
        }

        private IQueryable<AssetEntryEntity> GetSearchAssetEntriesQuery(AssetEntrySearchCriteria criteria, IPlatformRepository repository, IList<SortInfo> sortInfos)
        {
            var query = repository.AssetEntries;

            if (!string.IsNullOrEmpty(criteria.SearchPhrase))
            {
                query = query.Where(x =>
                    x.Name.Contains(criteria.SearchPhrase) || x.RelativeUrl.Contains(criteria.SearchPhrase));
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

            query = query.OrderBySortInfos(sortInfos);
            return query;
        }
    }
}
