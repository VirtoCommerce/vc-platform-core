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

        public PromotionUsageSearchService(IPlatformMemoryCache platformMemoryCache, Func<IMarketingRepository> repositoryFactory)
        {
            _platformMemoryCache = platformMemoryCache;
            _repositoryFactory = repositoryFactory;
        }

        public virtual async Task<GenericSearchResult<PromotionUsage>> SearchUsagesAsync(PromotionUsageSearchCriteria criteria)
        {
            if (criteria == null)
            {
                throw new ArgumentNullException(nameof(criteria));
            }

            var cacheKey = CacheKey.With(GetType(), "SearchUsagesAsync", criteria.GetCacheKey());
            return await _platformMemoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                cacheEntry.AddExpirationToken(PromotionUsageCacheRegion.CreateChangeToken());

                using (var repository = _repositoryFactory())
                {
                    var sortInfos = GetSortInfos(criteria);
                    var query = GetQuery(repository, criteria, sortInfos);


                    var totalCount = await query.CountAsync();
                    var searchResult = new GenericSearchResult<PromotionUsage> { TotalCount = totalCount };

                    if (criteria.Take > 0)
                    {
                        var coupons = await query.Skip(criteria.Skip).Take(criteria.Take).ToArrayAsync();
                        searchResult.Results = coupons.Select(x => x.ToModel(AbstractTypeFactory<PromotionUsage>.TryCreateInstance())).ToList();
                    }

                    return searchResult;
                }
            });
        }

        protected  virtual IList<SortInfo> GetSortInfos(PromotionUsageSearchCriteria criteria)
        {
            var sortInfos = criteria.SortInfos;
            if (sortInfos.IsNullOrEmpty())
            {
                sortInfos = new[]
                {
                    new SortInfo
                    {
                        SortColumn = ReflectionUtility.GetPropertyName<PromotionUsage>(x => x.ModifiedDate),
                        SortDirection = SortDirection.Descending
                    }
                };
            }

            return sortInfos;
        }

        protected virtual IQueryable<PromotionUsageEntity> GetQuery(IMarketingRepository repository, PromotionUsageSearchCriteria criteria, IList<SortInfo> sortInfos)
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

            query = query.OrderBySortInfos(sortInfos);
            return query;
        }
    }
}
