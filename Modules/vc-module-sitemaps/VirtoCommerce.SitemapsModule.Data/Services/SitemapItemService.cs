using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.SitemapsModule.Core.Models;
using VirtoCommerce.SitemapsModule.Core.Services;
using VirtoCommerce.SitemapsModule.Data.Models;
using VirtoCommerce.SitemapsModule.Data.Repositories;

namespace VirtoCommerce.SitemapsModule.Data.Services
{
    public class SitemapItemService : ISitemapItemService
    {
        public SitemapItemService(Func<ISitemapRepository> repositoryFactory)
        {
            RepositoryFactory = repositoryFactory;
        }

        protected Func<ISitemapRepository> RepositoryFactory { get; }

        public virtual async Task<GenericSearchResult<SitemapItem>> SearchAsync(SitemapItemSearchCriteria criteria)
        {
            if (criteria == null)
            {
                throw new ArgumentNullException(nameof(criteria));
            }

            // TODO: add caching

            using (var repository = RepositoryFactory())
            {
                var searchResponse = new GenericSearchResult<SitemapItem>();
                var query = repository.SitemapItems;
                if(!string.IsNullOrEmpty(criteria.SitemapId))
                {
                    query = query.Where(x => x.SitemapId == criteria.SitemapId);
                }
                if (criteria.ObjectTypes != null)
                {
                    query = query.Where(i => criteria.ObjectTypes.Contains(i.ObjectType, StringComparer.OrdinalIgnoreCase));
                }
                if (!string.IsNullOrEmpty(criteria.ObjectType))
                {
                    query = query.Where(i => i.ObjectType.EqualsInvariant(criteria.ObjectType));
                }

                var sortInfos = criteria.SortInfos;
                if (sortInfos.IsNullOrEmpty())
                {
                    sortInfos = new[] { new SortInfo { SortColumn = ReflectionUtility.GetPropertyName<SitemapItemEntity>(x => x.CreatedDate), SortDirection = SortDirection.Descending } };
                }
                query = query.OrderBySortInfos(sortInfos);
                searchResponse.TotalCount = await query.CountAsync();

                var matchingSitemapItems = await query.Skip(criteria.Skip).Take(criteria.Take).ToArrayAsync();
                foreach (var sitemapItemEntity in matchingSitemapItems)
                {
                    var sitemapItem = AbstractTypeFactory<SitemapItem>.TryCreateInstance();
                    if (sitemapItem != null)
                    {
                        searchResponse.Results.Add(sitemapItemEntity.ToModel(sitemapItem));
                    }
                }            
                return searchResponse;
            }
        }

        public virtual async Task SaveChangesAsync(SitemapItem[] sitemapItems)
        {
            if (sitemapItems == null)
            {
                throw new ArgumentNullException(nameof(sitemapItems));
            }

            using (var repository = RepositoryFactory())
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

            // TODO: add cache invalidation
        }

        public virtual async Task RemoveAsync(string[] itemIds)
        {
            if (itemIds == null)
            {
                throw new ArgumentNullException(nameof(itemIds));
            }

            using (var repository = RepositoryFactory())
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

            // TODO: add cache invalidation
        }
    }
}