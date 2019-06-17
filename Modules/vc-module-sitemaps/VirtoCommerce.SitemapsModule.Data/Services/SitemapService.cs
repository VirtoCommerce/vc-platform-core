using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.SitemapsModule.Core.Models;
using VirtoCommerce.SitemapsModule.Core.Services;
using VirtoCommerce.SitemapsModule.Data.Models;
using VirtoCommerce.SitemapsModule.Data.Repositories;

namespace VirtoCommerce.SitemapsModule.Data.Services
{
    public class SitemapService : ISitemapService
    {
        private readonly Func<ISitemapRepository> _repositoryFactory;
        private readonly ISitemapItemService _sitemapItemService;
        private readonly IPlatformMemoryCache _platformMemoryCache;

        public SitemapService(Func<ISitemapRepository> repositoryFactory, ISitemapItemService sitemapItemService, IPlatformMemoryCache platformMemoryCache)
        {
            _repositoryFactory = repositoryFactory;
            _sitemapItemService = sitemapItemService;
            _platformMemoryCache = platformMemoryCache;
        }

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

            using (var repository = _repositoryFactory())
            {
                var sitemapEntities = await repository.GetSitemapsAsync(sitemapIds, responseGroup);
                return sitemapEntities.Select(x =>
                {
                    var sitemap = x.ToModel(AbstractTypeFactory<Sitemap>.TryCreateInstance());
                    return sitemap;
                }).ToArray();
            }

        }


        public virtual async Task SaveChangesAsync(Sitemap[] sitemaps)
        {
            if (sitemaps == null)
            {
                throw new ArgumentNullException(nameof(sitemaps));
            }

            var pkMap = new PrimaryKeyResolvingMap();

            using (var repository = _repositoryFactory())
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
        }

        public virtual async Task RemoveAsync(string[] ids)
        {
            if (ids == null)
            {
                throw new ArgumentNullException(nameof(ids));
            }

            using (var repository = _repositoryFactory())
            {
                var sitemapEntities = await repository.GetSitemapsAsync(ids, SitemapResponseGroup.Full.ToString());
                foreach (var sitemapEntity in sitemapEntities)
                {
                    repository.Remove(sitemapEntity);
                }
                await repository.UnitOfWork.CommitAsync();
            }
        }

    }
}
