using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.StoreModule.Core.Model;
using VirtoCommerce.StoreModule.Core.Model.Search;
using VirtoCommerce.StoreModule.Core.Services;
using VirtoCommerce.StoreModule.Data.Caching;
using VirtoCommerce.StoreModule.Data.Model;
using VirtoCommerce.StoreModule.Data.Repositories;

namespace VirtoCommerce.StoreModule.Data.Services
{
    public class StoreSearchService : IStoreSearchService
    {
        public StoreSearchService(Func<IStoreRepository> repositoryFactory, IPlatformMemoryCache platformMemoryCache, IStoreService storeService)
        {
            RepositoryFactory = repositoryFactory;
            PlatformMemoryCache = platformMemoryCache;
            StoreService = storeService;
        }

        protected Func<IStoreRepository> RepositoryFactory { get; }
        protected IPlatformMemoryCache PlatformMemoryCache { get; }
        protected IStoreService StoreService { get; }

        public virtual async Task<GenericSearchResult<Store>> SearchStoresAsync(StoreSearchCriteria criteria)
        {
            var cacheKey = CacheKey.With(GetType(), "SearchStoresAsync", criteria.GetCacheKey());
            return await PlatformMemoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                cacheEntry.AddExpirationToken(StoreSearchCacheRegion.CreateChangeToken());
                var result = new GenericSearchResult<Store>();
                using (var repository = RepositoryFactory())
                {
                    var sortInfos = criteria.SortInfos;
                    if (sortInfos.IsNullOrEmpty())
                    {
                        sortInfos = new[]
                        {
                            new SortInfo
                            {
                                SortColumn = "Name"
                            }
                        };
                    }

                    var query = GetStoresQuery(repository, criteria, sortInfos);

                    result.TotalCount = await query.CountAsync();
                    if (criteria.Take > 0)
                    {
                        var storeIds = await query.Select(x => x.Id).Skip(criteria.Skip).Take(criteria.Take).ToArrayAsync();
                        result.Results = (await StoreService.GetByIdsAsync(storeIds)).AsQueryable().OrderBySortInfos(sortInfos).ToList();
                    }
                }
                return result;
            });
        }

        protected virtual IQueryable<StoreEntity> GetStoresQuery(IStoreRepository repository, StoreSearchCriteria criteria,
            IEnumerable<SortInfo> sortInfos)
        {
            var query = repository.Stores;
            if (!string.IsNullOrEmpty(criteria.Keyword))
            {
                query = query.Where(x => x.Name.Contains(criteria.Keyword) || x.Id.Contains(criteria.Keyword));
            }
            if (!criteria.StoreIds.IsNullOrEmpty())
            {
                query = query.Where(x => criteria.StoreIds.Contains(x.Id));
            }

            query = query.OrderBySortInfos(sortInfos);
            return query;
        }
    }
}
