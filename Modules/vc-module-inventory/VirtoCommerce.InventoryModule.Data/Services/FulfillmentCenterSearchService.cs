using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using VirtoCommerce.InventoryModule.Core.Model;
using VirtoCommerce.InventoryModule.Core.Model.Search;
using VirtoCommerce.InventoryModule.Core.Services;
using VirtoCommerce.InventoryModule.Data.Cashing;
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

        public FulfillmentCenterSearchService(Func<IInventoryRepository> repositoryFactory, IPlatformMemoryCache platformMemoryCache)
        {
            _repositoryFactory = repositoryFactory;
            _platformMemoryCache = platformMemoryCache;
        }

        public async Task<GenericSearchResult<FulfillmentCenter>> SearchCentersAsync(FulfillmentCenterSearchCriteria criteria)
        {
            var cacheKey = CacheKey.With(GetType(), "SearchCentersAsync", criteria.GetHashCode().ToString());
            return await _platformMemoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                cacheEntry.AddExpirationToken(FulfillmentCenterCacheRegion.CreateChangeToken());
                var result = new GenericSearchResult<FulfillmentCenter>();
                using (var repository = _repositoryFactory())
                {
                    repository.DisableChangesTracking();

                    var query = repository.FulfillmentCenters;
                    if (!string.IsNullOrEmpty(criteria.Keyword))
                    {
                        query = query.Where(x => x.Name.Contains(criteria.Keyword));
                    }

                    var sortInfos = criteria.SortInfos;
                    if (sortInfos.IsNullOrEmpty())
                    {
                        sortInfos = new[] { new SortInfo { SortColumn = "Name" } };
                    }

                    query = query.OrderBySortInfos(sortInfos);

                    result.TotalCount = await query.CountAsync();
                    var arrayFullfillmentCenters = await query.Skip(criteria.Skip).Take(criteria.Take).ToArrayAsync();
                    result.Results = arrayFullfillmentCenters
                        .Select(x => x.ToModel(AbstractTypeFactory<FulfillmentCenter>.TryCreateInstance()))
                        .ToList();
                }
                return result;
            });
        }
    }
}
