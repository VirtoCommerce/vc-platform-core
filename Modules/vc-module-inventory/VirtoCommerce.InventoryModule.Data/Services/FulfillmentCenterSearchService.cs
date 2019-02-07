using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using VirtoCommerce.InventoryModule.Core.Model;
using VirtoCommerce.InventoryModule.Core.Model.Search;
using VirtoCommerce.InventoryModule.Core.Services;
using VirtoCommerce.InventoryModule.Data.Caching;
using VirtoCommerce.InventoryModule.Data.Model;
using VirtoCommerce.InventoryModule.Data.Repositories;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Data.Infrastructure;

namespace VirtoCommerce.InventoryModule.Data.Services
{
    public class FulfillmentCenterSearchService : IFulfillmentCenterSearchService
    {
        public FulfillmentCenterSearchService(Func<IInventoryRepository> repositoryFactory, IPlatformMemoryCache platformMemoryCache)
        {
            RepositoryFactory = repositoryFactory;
            PlatformMemoryCache = platformMemoryCache;
        }

        protected Func<IInventoryRepository> RepositoryFactory { get; }
        protected IPlatformMemoryCache PlatformMemoryCache { get; }

        public virtual async Task<GenericSearchResult<FulfillmentCenter>> SearchCentersAsync(FulfillmentCenterSearchCriteria criteria)
        {
            var cacheKey = CacheKey.With(GetType(), "SearchCentersAsync", criteria.GetCacheKey());
            return await PlatformMemoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                cacheEntry.AddExpirationToken(FulfillmentCenterCacheRegion.CreateChangeToken());
                var result = new GenericSearchResult<FulfillmentCenter>();
                using (var repository = RepositoryFactory())
                {
                    repository.DisableChangesTracking();

                    var sortInfos = GetSortInfos(criteria);
                    var query = GetFulfillmentCenterQuery(repository, criteria, sortInfos);

                    result.TotalCount = await query.CountAsync();

                    var fulfillmentCenterEntities = new FulfillmentCenterEntity[0];
                    if (criteria.Take > 0)
                    {
                        fulfillmentCenterEntities = await query.Skip(criteria.Skip).Take(criteria.Take).ToArrayAsync();
                    }

                    result.Results = fulfillmentCenterEntities
                        .Select(x => x.ToModel(AbstractTypeFactory<FulfillmentCenter>.TryCreateInstance()))
                        .ToList();
                }
                return result;
            });
        }

        protected virtual IQueryable<FulfillmentCenterEntity> GetFulfillmentCenterQuery(IInventoryRepository repository,
            FulfillmentCenterSearchCriteria criteria, IEnumerable<SortInfo> sortInfos)
        {
            var query = repository.FulfillmentCenters;
            if (!string.IsNullOrEmpty(criteria.Keyword))
            {
                query = query.Where(x => x.Name.Contains(criteria.Keyword));
            }

            return query.OrderBySortInfos(sortInfos);
        }

        protected virtual IList<SortInfo> GetSortInfos(FulfillmentCenterSearchCriteria criteria)
        {
            var sortInfos = criteria.SortInfos;
            if (sortInfos.IsNullOrEmpty())
            {
                sortInfos = new[] { new SortInfo { SortColumn = "Name" } };
            }

            return sortInfos;
        }
    }
}
