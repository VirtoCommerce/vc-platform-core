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
    public class CouponSearchService: ICouponSearchService
    {
        private readonly Func<IMarketingRepository> _repositoryFactory;
        private readonly IPlatformMemoryCache _platformMemoryCache;
        private readonly ICouponService _couponService;

        public CouponSearchService(Func<IMarketingRepository> repositoryFactory, IPlatformMemoryCache platformMemoryCache, ICouponService couponService)
        {
            _repositoryFactory = repositoryFactory;
            _platformMemoryCache = platformMemoryCache;
            _couponService = couponService;
        }

        public async Task<CouponSearchResult> SearchCouponsAsync(CouponSearchCriteria criteria)
        {
            if (criteria == null)
            {
                throw new ArgumentNullException(nameof(criteria));
            }

            var cacheKey = CacheKey.With(GetType(), nameof(SearchCouponsAsync), criteria.GetCacheKey());
            return await _platformMemoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                cacheEntry.AddExpirationToken(CouponCacheRegion.CreateChangeToken());

                using (var repository = _repositoryFactory())
                {
                    var sortInfos = BuildSearchExpression(criteria);
                    var query = BuildQuery(criteria, repository);

                    var totalCount = await query.CountAsync();
                    var searchResult = AbstractTypeFactory<CouponSearchResult>.TryCreateInstance();
                    searchResult.TotalCount = totalCount;

                    if (criteria.Take > 0)
                    {
                        var ids = await query.OrderBySortInfos(sortInfos).ThenBy(x => x.Id)
                                             .Select(x => x.Id)
                                             .Skip(criteria.Skip).Take(criteria.Take)
                                             .ToArrayAsync();

                        searchResult.Results = (await _couponService.GetByIdsAsync(ids)).OrderBy(x => Array.IndexOf(ids, x.Id)).ToList();
                    }

                    return searchResult;
                }
            });
        }

        protected virtual IQueryable<CouponEntity> BuildQuery(CouponSearchCriteria criteria, IMarketingRepository repository)
        {
            var query = repository.Coupons;

            if (!string.IsNullOrEmpty(criteria.PromotionId))
            {
                query = query.Where(c => c.PromotionId == criteria.PromotionId);
            }

            if (!string.IsNullOrEmpty(criteria.Code))
            {
                query = query.Where(c => c.Code == criteria.Code);
            }

            if (!criteria.Codes.IsNullOrEmpty())
            {
                query = query.Where(c => criteria.Codes.Contains(c.Code));
            }

            return query;
        }

        protected virtual IList<SortInfo> BuildSearchExpression(CouponSearchCriteria criteria)
        {
            var sortInfos = criteria.SortInfos;
            //TODO: Sort by TotalUsesCount 
            if (sortInfos.IsNullOrEmpty() || sortInfos.Any(x => x.SortColumn.EqualsInvariant(nameof(Coupon.TotalUsesCount))))
            {
                sortInfos = new[]
                {
                    new SortInfo
                    {
                        SortColumn = nameof(Coupon.Code),
                        SortDirection = SortDirection.Descending
                    }
                }.ToList();
            }

            if (sortInfos.Count < 2)
            {
                sortInfos.Add(new SortInfo
                {
                    SortColumn = nameof(Coupon.Id),
                    SortDirection = SortDirection.Ascending
                });
            }

            return sortInfos;
        }
    }
}
