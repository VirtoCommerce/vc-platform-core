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
    public class SitemapItemService : ISitemapItemService
    {
        private readonly Func<ISitemapRepository> _repositoryFactory;
        private readonly IPlatformMemoryCache _platformMemoryCache;

        public SitemapItemService(Func<ISitemapRepository> repositoryFactory, IPlatformMemoryCache platformMemoryCache)
        {
            _repositoryFactory = repositoryFactory;
            _platformMemoryCache = platformMemoryCache;
        }


        public async Task<IEnumerable<SitemapItem>> GetByIdsAsync(string[] itemIds, string responseGroup = null)
        {
            if (itemIds == null)
            {
                throw new ArgumentNullException(nameof(itemIds));
            }

            using (var repository = _repositoryFactory())
            {
                var sitemapEntities = await repository.GetSitemapItemsAsync(itemIds);
                return sitemapEntities.Select(x => x.ToModel(AbstractTypeFactory<SitemapItem>.TryCreateInstance())).ToArray();
            }
        }

        public virtual async Task SaveChangesAsync(SitemapItem[] sitemapItems)
        {
            if (sitemapItems == null)
            {
                throw new ArgumentNullException(nameof(sitemapItems));
            }

            using (var repository = _repositoryFactory())
            {
                var pkMap = new PrimaryKeyResolvingMap();
                var itemsIds = sitemapItems.Select(x => x.Id).Where(x => x != null).Distinct().ToArray();
                var existEntities = await repository.SitemapItems.Where(s => itemsIds.Contains(s.Id)).ToArrayAsync();
                foreach (var sitemapItem in sitemapItems)
                {
                    var changedEntity = AbstractTypeFactory<SitemapItemEntity>.TryCreateInstance().FromModel(sitemapItem, pkMap);
                    var existEntity = existEntities.FirstOrDefault(x => x.Id == sitemapItem.Id);
                    if (existEntity != null)
                    {
                        changedEntity.Patch(existEntity);
                    }
                    else
                    {
                        repository.Add(changedEntity);
                    }
                }

                await repository.UnitOfWork.CommitAsync();
                pkMap.ResolvePrimaryKeys();
            }
        }

        public virtual async Task RemoveAsync(string[] itemIds)
        {
            if (itemIds == null)
            {
                throw new ArgumentNullException(nameof(itemIds));
            }

            using (var repository = _repositoryFactory())
            {
                var sitemapItemEntities = await repository.SitemapItems.Where(i => itemIds.Contains(i.Id)).ToArrayAsync();
                if (sitemapItemEntities.Any())
                {
                    foreach (var sitemapItemEntity in sitemapItemEntities)
                    {
                        repository.Remove(sitemapItemEntity);
                    }
                    await repository.UnitOfWork.CommitAsync();
                }
            }
        }

    }
}
