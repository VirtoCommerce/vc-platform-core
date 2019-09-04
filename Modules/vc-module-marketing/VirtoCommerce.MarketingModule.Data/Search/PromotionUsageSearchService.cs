using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using VirtoCommerce.MarketingModule.Core.Model.Promotions;
using VirtoCommerce.MarketingModule.Core.Model.Promotions.Search;
using VirtoCommerce.MarketingModule.Core.Search;
using VirtoCommerce.MarketingModule.Data.Caching;
using VirtoCommerce.MarketingModule.Data.Model;
using VirtoCommerce.MarketingModule.Data.Repositories;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.MarketingModule.Data.Search
{
    public class PromotionUsageSearchService: IPromotionUsageSearchService
    {
        private readonly IPlatformMemoryCache _platformMemoryCache;
        private readonly Func<IMarketingRepository> _repositoryFactory;

        public PromotionUsageSearchService(Func<IMarketingRepository> repositoryFactory, IPlatformMemoryCache platformMemoryCache)
        {
            _platformMemoryCache = platformMemoryCache;
            _repositoryFactory = repositoryFactory;
        }

        public virtual async Task<PromotionUsageSearchResult> SearchUsagesAsync(PromotionUsageSearchCriteria criteria)
        {
            if (criteria == null)
            {
                throw new ArgumentNullException(nameof(criteria));
            }

            var cacheKey = CacheKey.With(GetType(), nameof(SearchUsagesAsync), criteria.GetCacheKey());
            return await _platformMemoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                var result = AbstractTypeFactory<PromotionUsageSearchResult>.TryCreateInstance();
              
                using (var repository = _repositoryFactory())
                {
                    var sortInfos = BuildSortExpression(criteria);
                    var query = BuildQuery(repository, criteria);

                    result.TotalCount = await query.CountAsync();

                    if (criteria.Take > 0)
                    {
                        var usages = await query.OrderBySortInfos(sortInfos).ThenBy(x => x.Id)
                                         .Skip(criteria.Skip).Take(criteria.Take)
                                         .ToArrayAsync();
                        result.Results = usages.Select(x => x.ToModel(AbstractTypeFactory<PromotionUsage>.TryCreateInstance())).ToList();
                        foreach(var usage in result.Results)
                        {
                            cacheEntry.AddExpirationToken(PromotionUsageCacheRegion.CreateChangeToken(usage));
                        }
                    }
                    return result;
                }
            });
        }

        protected  virtual IList<SortInfo> BuildSortExpression(PromotionUsageSearchCriteria criteria)
        {
            var sortInfos = criteria.SortInfos;
            if (sortInfos.IsNullOrEmpty())
            {
                sortInfos = new[]
                {
                    new SortInfo
                    {
                        SortColumn = nameof(PromotionUsage.ModifiedDate),
                        SortDirection = SortDirection.Descending
                    }
                };
            }

            return sortInfos;
        }

        protected virtual IQueryable<PromotionUsageEntity> BuildQuery(IMarketingRepository repository, PromotionUsageSearchCriteria criteria)
        {
            var query = repository.PromotionUsages;

            if (!string.IsNullOrEmpty(criteria.PromotionId))
            {
                query = query.Where(x => x.PromotionId == criteria.PromotionId);
            }
            if (!string.IsNullOrEmpty(criteria.CouponCode))
            {
                query = query.Where(x => x.CouponCode == criteria.CouponCode);
            }
            if (!string.IsNullOrEmpty(criteria.ObjectId))
            {
                query = query.Where(x => x.ObjectId == criteria.ObjectId);
            }
            if (!string.IsNullOrEmpty(criteria.ObjectType))
            {
                query = query.Where(x => x.ObjectType == criteria.ObjectType);
            }
            if (!string.IsNullOrWhiteSpace(criteria.UserId))
            {
                query = query.Where(x => x.UserId == criteria.UserId);
            }
            if (!string.IsNullOrWhiteSpace(criteria.UserName))
            {
                query = query.Where(x => x.UserName == criteria.UserName);
            }

            return query;
        }
    }
}
