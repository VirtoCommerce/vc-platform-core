using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using VirtoCommerce.MarketingModule.Core.Model.Promotions;
using VirtoCommerce.MarketingModule.Core.Model.Promotions.Search;
using VirtoCommerce.MarketingModule.Core.Search;
using VirtoCommerce.MarketingModule.Core.Services;
using VirtoCommerce.MarketingModule.Data.Caching;
using VirtoCommerce.MarketingModule.Data.Model;
using VirtoCommerce.MarketingModule.Data.Repositories;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.MarketingModule.Data.Search
{
    public class PromotionSearchService: IPromotionSearchService
    {
        private readonly Func<IMarketingRepository> _repositoryFactory;
        private readonly IPromotionService _promotionService;
        private readonly IPlatformMemoryCache _platformMemoryCache;

        public PromotionSearchService(Func<IMarketingRepository> repositoryFactory ,IPromotionService promotionService, IPlatformMemoryCache platformMemoryCache)
        {
            _repositoryFactory = repositoryFactory;
            _promotionService = promotionService;
            _platformMemoryCache = platformMemoryCache;
        }

        public virtual async Task<PromotionSearchResult> SearchPromotionsAsync(PromotionSearchCriteria criteria)
        {
            var cacheKey = CacheKey.With(GetType(), nameof(SearchPromotionsAsync), criteria.GetCacheKey());
            return await _platformMemoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                cacheEntry.AddExpirationToken(PromotionCacheRegion.CreateChangeToken());
                var retVal = AbstractTypeFactory<PromotionSearchResult>.TryCreateInstance();
                using (var repository = _repositoryFactory())
                {
                    var sortInfos = BuildSortExpression(criteria);
                    var query = BuildQuery(repository, criteria);
                    //https://github.com/zzzprojects/EntityFramework-Plus/issues/293
                    //Workaround ArgumentException with CountAsync have no predicate when query has a Cast or OfType expression
                    retVal.TotalCount = await query.CountAsync(x=> true);

                    if (criteria.Take > 0)
                    {
                        var ids = await query.OrderBySortInfos(sortInfos).ThenBy(x => x.Id)
                                        .Select(x => x.Id)
                                        .Skip(criteria.Skip).Take(criteria.Take)
                                        .ToArrayAsync();

                        retVal.Results = (await _promotionService.GetPromotionsByIdsAsync(ids))
                            .OrderBy(x => Array.IndexOf(ids, x.Id)).ToList();
                    }
                }
                return retVal;
            });
        }

        protected virtual IList<SortInfo> BuildSortExpression(PromotionSearchCriteria criteria)
        {
            var sortInfos = criteria.SortInfos;
            if (sortInfos.IsNullOrEmpty())
            {
                sortInfos = new[]
                {
                    new SortInfo
                    {
                        SortColumn = nameof(Promotion.Priority),
                        SortDirection = SortDirection.Descending
                    }
                };
            }

            return sortInfos;
        }

        protected virtual IQueryable<PromotionEntity> BuildQuery(IMarketingRepository repository, PromotionSearchCriteria criteria)
        {
            var query = repository.Promotions;

            if (!string.IsNullOrEmpty(criteria.Store))
            {
                query = query.Where(x => !x.Stores.Any() || x.Stores.Any(s => s.StoreId == criteria.Store));
            }

            if (!criteria.StoreIds.IsNullOrEmpty())
            {
                query = query.Where(x => !x.Stores.Any() || x.Stores.Any(s => criteria.StoreIds.Contains(s.StoreId)));
            }

            if (criteria.OnlyActive)
            {
                var now = DateTime.UtcNow;
                query = query.Where(x => x.IsActive && (x.StartDate == null || now >= x.StartDate) && (x.EndDate == null || x.EndDate >= now));
            }

            if (!string.IsNullOrEmpty(criteria.Keyword))
            {
                query = query.Where(x => x.Name.Contains(criteria.Keyword) || x.Description.Contains(criteria.Keyword));
            }
            return query;
        }
    }
}
