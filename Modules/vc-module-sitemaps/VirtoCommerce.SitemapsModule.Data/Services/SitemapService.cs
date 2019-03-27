using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.SitemapsModule.Core.Models;
using VirtoCommerce.SitemapsModule.Core.Services;
using VirtoCommerce.SitemapsModule.Data.Caching;
using VirtoCommerce.SitemapsModule.Data.Models;
using VirtoCommerce.SitemapsModule.Data.Repositories;

namespace VirtoCommerce.SitemapsModule.Data.Services
{
    public class SitemapService : ISitemapService
    {
        public SitemapService(Func<ISitemapRepository> repositoryFactory, ISitemapItemService sitemapItemService, IPlatformMemoryCache platformMemoryCache)
        {
            RepositoryFactory = repositoryFactory;
            SitemapItemService = sitemapItemService;
            PlatformMemoryCache = platformMemoryCache;
        }

        protected Func<ISitemapRepository> RepositoryFactory { get; }
        protected ISitemapItemService SitemapItemService { get; }
        protected IPlatformMemoryCache PlatformMemoryCache { get; }

        public virtual async Task<Sitemap> GetByIdAsync(string sitemapId, string responseGroup = null)
        {
            return (await GetByIdsAsync(new[] { sitemapId }, responseGroup)).FirstOrDefault();
        }

        public virtual async Task<IEnumerable<Sitemap>> GetByIdsAsync(string[] sitemapIds, string responseGroup = null)
        {
            if (sitemapIds == null)
            {
                throw new ArgumentNullException(nameof(sitemapIds));
            }

            var cacheKey = CacheKey.With(GetType(), nameof(GetByIdsAsync), string.Join("-", sitemapIds.OrderBy(x => x)), responseGroup);
            return await PlatformMemoryCache.GetOrCreateExclusiveAsync(cacheKey, async cacheEntry =>
            {
                using (var repository = RepositoryFactory())
                {
                    var sitemapEntities = await repository.GetSitemapsAsync(sitemapIds, responseGroup);
                    return sitemapEntities.Select(x =>
                    {
                        var sitemap = x.ToModel(AbstractTypeFactory<Sitemap>.TryCreateInstance());
                        cacheEntry.AddExpirationToken(SitemapCacheRegion.CreateChangeToken(x.Id));
                        return sitemap;
                    }).ToArray();
                }
            });
        }

        public virtual async Task<GenericSearchResult<Sitemap>> SearchAsync(SitemapSearchCriteria criteria)
        {
            if (criteria == null)
            {
                throw new ArgumentNullException(nameof(criteria));
            }

            var cacheKey = CacheKey.With(GetType(), nameof(SearchAsync), criteria.GetCacheKey());
            return await PlatformMemoryCache.GetOrCreateExclusiveAsync(cacheKey, async cacheEntry =>
            {
                cacheEntry.AddExpirationToken(SitemapSearchCacheRegion.CreateChangeToken());

                using (var repository = RepositoryFactory())
                {
                    var result = new GenericSearchResult<Sitemap>();

                    var sortInfos = GetSearchSortInfo(criteria);
                    var query = GetSearchQuery(criteria, repository, sortInfos);

                    result.TotalCount = await query.CountAsync();

                    if (criteria.Take > 0)
                    {
                        var sitemapIds = await query.Select(x => x.Id).Skip(criteria.Skip).Take(criteria.Take).ToArrayAsync();
                        result.Results = (await GetByIdsAsync(sitemapIds, criteria.ResponseGroup)).AsQueryable().OrderBySortInfos(sortInfos).ToArray();
                    }

                    return result;
                }
            });
        }

        public virtual async Task SaveChangesAsync(Sitemap[] sitemaps)
        {
            if (sitemaps == null)
            {
                throw new ArgumentNullException(nameof(sitemaps));
            }

            var pkMap = new PrimaryKeyResolvingMap();

            using (var repository = RepositoryFactory())
            {
                var sitemapIds = sitemaps.Where(s => !s.IsTransient()).Select(s => s.Id);
                var sitemapExistEntities = await repository.Sitemaps.Where(s => sitemapIds.Contains(s.Id)).ToArrayAsync();
                foreach (var sitemap in sitemaps)
                {
                    var sitemapSourceEntity = AbstractTypeFactory<SitemapEntity>.TryCreateInstance();
                    if (sitemapSourceEntity != null)
                    {
                        sitemapSourceEntity.FromModel(sitemap, pkMap);
                        var sitemapTargetEntity = sitemapExistEntities.FirstOrDefault(s => s.Id == sitemap.Id);
                        if (sitemapTargetEntity != null)
                        {
                            sitemapSourceEntity.Patch(sitemapTargetEntity);
                        }
                        else
                        {
                            repository.Add(sitemapSourceEntity);
                        }
                    }
                }

                await repository.UnitOfWork.CommitAsync();
                pkMap.ResolvePrimaryKeys();
            }

            ClearCacheFor(sitemaps.Select(x => x.Id));
        }

        public virtual async Task RemoveAsync(string[] ids)
        {
            if (ids == null)
            {
                throw new ArgumentNullException(nameof(ids));
            }

            using (var repository = RepositoryFactory())
            {
                var sitemapEntities = await repository.GetSitemapsAsync(ids, SitemapResponseGroup.Full.ToString());
                foreach (var sitemapEntity in sitemapEntities)
                {
                    repository.Remove(sitemapEntity);
                }
                await repository.UnitOfWork.CommitAsync();
            }
            ClearCacheFor(ids);
        }

        protected virtual void ClearCacheFor(IEnumerable<string> sitemapIds)
        {
            foreach (var sitemapId in sitemapIds)
            {
                SitemapCacheRegion.ExpireSitemap(sitemapId);
            }
            SitemapSearchCacheRegion.ExpireRegion();
        }

        protected virtual IList<SortInfo> GetSearchSortInfo(SitemapSearchCriteria criteria)
        {
            var sortInfos = criteria.SortInfos;
            if (sortInfos.IsNullOrEmpty())
            {
                sortInfos = new[]
                {
                    new SortInfo
                    {
                        SortColumn = ReflectionUtility.GetPropertyName<SitemapEntity>(x => x.CreatedDate),
                        SortDirection = SortDirection.Descending
                    }
                };
            }

            return sortInfos;
        }

        protected virtual IQueryable<SitemapEntity> GetSearchQuery(SitemapSearchCriteria criteria, ISitemapRepository repository, IList<SortInfo> sortInfos)
        {
            var query = repository.Sitemaps;

            if (!string.IsNullOrEmpty(criteria.StoreId))
            {
                query = query.Where(s => s.StoreId == criteria.StoreId);
            }

            if (!string.IsNullOrEmpty(criteria.Location))
            {
                query = query.Where(s => s.Filename == criteria.Location);
            }

            query = query.OrderBySortInfos(sortInfos);
            return query;
        }
    }
}
