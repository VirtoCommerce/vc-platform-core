using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Operations;
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
        public SitemapService(
            Func<ISitemapRepository> repositoryFactory,
            ISitemapItemService sitemapItemService,
            IPlatformMemoryCache platformMemoryCache)
        {
            RepositoryFactory = repositoryFactory;
            SitemapItemService = sitemapItemService;
            PlatformMemoryCache = platformMemoryCache;
        }

        protected Func<ISitemapRepository> RepositoryFactory { get; }
        protected ISitemapItemService SitemapItemService { get; }
        protected IPlatformMemoryCache PlatformMemoryCache { get; }

        public virtual async Task<Sitemap> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            var cacheKey = CacheKey.With(GetType(), nameof(GetByIdAsync), id);
            return await PlatformMemoryCache.GetOrCreateExclusiveAsync(cacheKey, async cacheEntry =>
            {
                using (var repository = RepositoryFactory())
                {
                    Sitemap sitemap = null;

                    var sitemapEntity = await repository.Sitemaps.FirstOrDefaultAsync(s => s.Id == id);
                    if (sitemapEntity != null)
                    {
                        sitemap = AbstractTypeFactory<Sitemap>.TryCreateInstance();
                        if (sitemap != null)
                        {
                            var sitemapItemsSearchResponse = await SitemapItemService.SearchAsync(
                                new SitemapItemSearchCriteria
                                {
                                    SitemapId = sitemap.Id
                                });

                            sitemap = sitemapEntity.ToModel(sitemap);
                            // sitemap.Items = sitemapItemsSearchResponse.Results;
                            sitemap.TotalItemsCount = sitemapItemsSearchResponse.TotalCount;

                            cacheEntry.AddExpirationToken(SitemapCacheRegion.CreateChangeToken(id));
                        }
                    }

                    return sitemap;
                }
            });
        }

        public virtual async Task<GenericSearchResult<Sitemap>> SearchAsync(SitemapSearchCriteria request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var cacheKey = CacheKey.With(GetType(), nameof(SearchAsync), request.GetCacheKey());
            return await PlatformMemoryCache.GetOrCreateExclusiveAsync(cacheKey, async cacheEntry =>
            {
                cacheEntry.AddExpirationToken(SitemapSearchCacheRegion.CreateChangeToken());

                using (var repository = RepositoryFactory())
                {
                    var searchResponse = new GenericSearchResult<Sitemap>();

                    var sitemapEntities = repository.Sitemaps;

                    if (!string.IsNullOrEmpty(request.StoreId))
                    {
                        sitemapEntities = sitemapEntities.Where(s => s.StoreId == request.StoreId);
                    }

                    if (!string.IsNullOrEmpty(request.Location))
                    {
                        sitemapEntities = sitemapEntities.Where(s => s.Filename == request.Location);
                    }

                    searchResponse.TotalCount = await sitemapEntities.CountAsync();

                    var matchingEntities = await sitemapEntities.OrderByDescending(s => s.CreatedDate)
                        .Skip(request.Skip).Take(request.Take).ToArrayAsync();
                    foreach (var sitemapEntity in matchingEntities)
                    {
                        var sitemap = AbstractTypeFactory<Sitemap>.TryCreateInstance();
                        if (sitemap != null)
                        {
                            var sitemapItemsSearchResponse = await SitemapItemService.SearchAsync(
                                new SitemapItemSearchCriteria
                                {
                                    SitemapId = sitemapEntity.Id
                                });

                            sitemap = sitemapEntity.ToModel(sitemap);
                            sitemap.TotalItemsCount = sitemapItemsSearchResponse.TotalCount;
                            searchResponse.Results.Add(sitemap);
                        }
                    }

                    return searchResponse;
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
                var sitemapEntities = await repository.Sitemaps.Where(s => ids.Contains(s.Id)).ToArrayAsync();
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
    }
}
