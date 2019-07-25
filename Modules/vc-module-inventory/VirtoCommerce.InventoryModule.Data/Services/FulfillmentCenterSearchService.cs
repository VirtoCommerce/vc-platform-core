using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
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
        private readonly Func<IInventoryRepository> _repositoryFactory;
        private readonly IPlatformMemoryCache _platformMemoryCache;
        private readonly IFulfillmentCenterService _fulfillmentService;

        public FulfillmentCenterSearchService(Func<IInventoryRepository> repositoryFactory, IFulfillmentCenterService fulfillmentService, IPlatformMemoryCache platformMemoryCache)
        {
            _repositoryFactory = repositoryFactory;
            _platformMemoryCache = platformMemoryCache;
            _fulfillmentService = fulfillmentService;
        }

        public virtual async Task<FulfillmentCenterSearchResult> SearchCentersAsync(FulfillmentCenterSearchCriteria criteria)
        {
            var cacheKey = CacheKey.With(GetType(), nameof(SearchCentersAsync), criteria.GetCacheKey());
            return await _platformMemoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                cacheEntry.AddExpirationToken(FulfillmentCenterCacheRegion.CreateChangeToken());
                var result = AbstractTypeFactory<FulfillmentCenterSearchResult>.TryCreateInstance();
                using (var repository = _repositoryFactory())
                {
                    repository.DisableChangesTracking();

                    var sortInfos = BuildSortExpression(criteria);
                    var query = BuildQuery(repository, criteria);

                    result.TotalCount = await query.CountAsync();
                    
                    if (criteria.Take > 0)
                    {
                        var ids = await query.OrderBySortInfos(sortInfos).ThenBy(x => x.Id)
                                           .Select(x => x.Id)
                                           .Skip(criteria.Skip).Take(criteria.Take)
                                           .ToArrayAsync();

                        result.Results = (await _fulfillmentService.GetByIdsAsync(ids)).OrderBy(x => Array.IndexOf(ids, x.Id)).ToList();
                    }                 
                }
                return result;
            });
        }

        protected virtual IQueryable<FulfillmentCenterEntity> BuildQuery(IInventoryRepository repository, FulfillmentCenterSearchCriteria criteria)
        {
            var query = repository.FulfillmentCenters;
            if (!string.IsNullOrEmpty(criteria.Keyword))
            {
                query = query.Where(x => x.Name.Contains(criteria.Keyword));
            }

            return query;
        }

        protected virtual IList<SortInfo> BuildSortExpression(FulfillmentCenterSearchCriteria criteria)
        {
            var sortInfos = criteria.SortInfos;
            if (sortInfos.IsNullOrEmpty())
            {
                sortInfos = new[] { new SortInfo { SortColumn = nameof(FulfillmentCenterEntity.Name) } };
            }

            return sortInfos;
        }
    }
}
