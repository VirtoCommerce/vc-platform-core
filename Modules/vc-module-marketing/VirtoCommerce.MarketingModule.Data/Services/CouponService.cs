using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using VirtoCommerce.MarketingModule.Core.Events;
using VirtoCommerce.MarketingModule.Core.Model.Promotions;
using VirtoCommerce.MarketingModule.Core.Model.Promotions.Search;
using VirtoCommerce.MarketingModule.Core.Services;
using VirtoCommerce.MarketingModule.Data.Caching;
using VirtoCommerce.MarketingModule.Data.Model;
using VirtoCommerce.MarketingModule.Data.Repositories;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Events;

namespace VirtoCommerce.MarketingModule.Data.Services
{
    public class CouponService : ICouponService
    {
        private readonly Func<IMarketingRepository> _repositoryFactory;
        private readonly IEventPublisher _eventPublisher;
        private readonly IPlatformMemoryCache _platformMemoryCache;

        public CouponService(Func<IMarketingRepository> repositoryFactory, IEventPublisher eventPublisher, IPlatformMemoryCache platformMemoryCache)
        {
            _repositoryFactory = repositoryFactory;
            _eventPublisher = eventPublisher;
            _platformMemoryCache = platformMemoryCache;
        }

        #region ICouponService members

        public async Task<GenericSearchResult<Coupon>> SearchCouponsAsync(CouponSearchCriteria criteria)
        {
            if (criteria == null)
            {
                throw new ArgumentNullException(nameof(criteria));
            }

            var cacheKey = CacheKey.With(GetType(), "SearchCouponsAsync", criteria.GetCacheKey());
            return await _platformMemoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                cacheEntry.AddExpirationToken(CouponCacheRegion.CreateChangeToken());

                using (var repository = _repositoryFactory())
                {
                    var sortInfos = GetSortInfos(criteria);
                    var query = GetQuery(criteria, repository, sortInfos);

                    var totalCount = await query.CountAsync();
                    var searchResult = new GenericSearchResult<Coupon> { TotalCount = totalCount };

                    if (criteria.Take > 0)
                    {
                        var ids = await query.Select(x => x.Id).Skip(criteria.Skip).Take(criteria.Take).ToArrayAsync();
                        searchResult.Results = (await GetByIdsAsync(ids)).OrderBy(x => Array.IndexOf(ids, x.Id)).ToList();
                    }

                    return searchResult;
                }
            });
        }

        public async Task<Coupon[]> GetByIdsAsync(string[] ids)
        {
            var cacheKey = CacheKey.With(GetType(), "GetByIdsAsync", string.Join("-", ids));
            return await _platformMemoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                cacheEntry.AddExpirationToken(CouponCacheRegion.CreateChangeToken());
                using (var repository = _repositoryFactory())
                {
                    var coupons = await repository.GetCouponsByIdsAsync(ids);
                    return coupons.Select(x => x.ToModel(AbstractTypeFactory<Coupon>.TryCreateInstance())).ToArray();
                }
            });
        }

        public async Task SaveCouponsAsync(Coupon[] coupons)
        {
            var pkMap = new PrimaryKeyResolvingMap();
            var changedEntries = new List<GenericChangedEntry<Coupon>>();
            using (var repository = _repositoryFactory())
            {
                var existCouponEntities = await repository.GetCouponsByIdsAsync(coupons.Where(x => !x.IsTransient()).Select(x => x.Id).ToArray());
                foreach (var coupon in coupons)
                {
                    var sourceEntity = AbstractTypeFactory<CouponEntity>.TryCreateInstance();
                    if (sourceEntity != null)
                    {
                        sourceEntity = sourceEntity.FromModel(coupon, pkMap);
                        var targetCouponEntity = existCouponEntities.FirstOrDefault(x => x.Id == coupon.Id);
                        if (targetCouponEntity != null)
                        {
                            changedEntries.Add(new GenericChangedEntry<Coupon>(coupon, sourceEntity.ToModel(AbstractTypeFactory<Coupon>.TryCreateInstance()), EntryState.Modified));
                            sourceEntity.Patch(targetCouponEntity);
                        }
                        else
                        {
                            changedEntries.Add(new GenericChangedEntry<Coupon>(coupon, EntryState.Added));
                            repository.Add(sourceEntity);
                        }
                    }
                }
                await repository.UnitOfWork.CommitAsync();
                pkMap.ResolvePrimaryKeys();
                await _eventPublisher.Publish(new CouponChangedEvent(changedEntries));
            }

            CouponCacheRegion.ExpireRegion();
        }

        public async Task DeleteCouponsAsync(string[] ids)
        {
            using (var repository = _repositoryFactory())
            {
                await repository.RemoveCouponsAsync(ids);
                await repository.UnitOfWork.CommitAsync();
            }

            CouponCacheRegion.ExpireRegion();
        }

        #endregion

        protected virtual IQueryable<CouponEntity> GetQuery(CouponSearchCriteria criteria, IMarketingRepository repository, IList<SortInfo> sortInfos)
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

            query = query.OrderBySortInfos(sortInfos);

            return query;
        }

        protected virtual IList<SortInfo> GetSortInfos(CouponSearchCriteria criteria)
        {
            var sortInfos = criteria.SortInfos;
            //TODO: Sort by TotalUsesCount 
            if (sortInfos.IsNullOrEmpty()
                || sortInfos.Any(x => x.SortColumn.EqualsInvariant(ReflectionUtility.GetPropertyName<Coupon>(p => p.TotalUsesCount))))
            {
                sortInfos = new[]
                {
                    new SortInfo
                    {
                        SortColumn = ReflectionUtility.GetPropertyName<Coupon>(x => x.Code),
                        SortDirection = SortDirection.Descending
                    }
                };
            }

            if (sortInfos.Count < 2)
            {
                sortInfos.Add(new SortInfo
                {
                    SortColumn = ReflectionUtility.GetPropertyName<Coupon>(x => x.Id),
                    SortDirection = SortDirection.Ascending
                });
            }

            return sortInfos;
        }
    }
}
