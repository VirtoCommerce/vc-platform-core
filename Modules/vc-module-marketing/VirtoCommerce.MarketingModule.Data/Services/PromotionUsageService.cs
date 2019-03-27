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
    public class PromotionUsageService : IPromotionUsageService
    {
        private readonly Func<IMarketingRepository> _repositoryFactory;
        private readonly IEventPublisher _eventPublisher;
        private readonly IPlatformMemoryCache _platformMemoryCache;

        public PromotionUsageService(Func<IMarketingRepository> repositoryFactory, IEventPublisher eventPublisher, IPlatformMemoryCache platformMemoryCache)
        {
            _repositoryFactory = repositoryFactory;
            _eventPublisher = eventPublisher;
            _platformMemoryCache = platformMemoryCache;
        }

        #region IMarketingUsageService Members

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
                    var sortInfos = GetPromotionUsageSortInfo(criteria);
                    var query = GetPromotionUsageQuery(repository, criteria, sortInfos);

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

        public virtual async Task<PromotionUsage[]> GetByIdsAsync(string[] ids)
        {
            var cacheKey = CacheKey.With(GetType(), "GetByIdsAsync", string.Join("-", ids));
            return await _platformMemoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                cacheEntry.AddExpirationToken(PromotionUsageCacheRegion.CreateChangeToken());
                using (var repository = _repositoryFactory())
                {
                    var promotionUsage = await repository.GetMarketingUsagesByIdsAsync(ids);
                    return promotionUsage.Select(x => x.ToModel(AbstractTypeFactory<PromotionUsage>.TryCreateInstance())).ToArray();
                }
            });
        }

        public virtual async Task SaveUsagesAsync(PromotionUsage[] usages)
        {
            var pkMap = new PrimaryKeyResolvingMap();
            var changedEntries = new List<GenericChangedEntry<PromotionUsage>>();
            using (var repository = _repositoryFactory())
            {
                var existUsageEntities = await repository.GetMarketingUsagesByIdsAsync(usages.Where(x => !x.IsTransient()).Select(x => x.Id).ToArray());
                foreach (var usage in usages)
                {
                    var sourceEntity = AbstractTypeFactory<PromotionUsageEntity>.TryCreateInstance();
                    if (sourceEntity != null)
                    {
                        sourceEntity = sourceEntity.FromModel(usage, pkMap);
                        var targetUsageEntity = existUsageEntities.FirstOrDefault(x => x.Id == usage.Id);
                        if (targetUsageEntity != null)
                        {
                            changedEntries.Add(new GenericChangedEntry<PromotionUsage>(usage, sourceEntity.ToModel(AbstractTypeFactory<PromotionUsage>.TryCreateInstance()), EntryState.Modified));
                            sourceEntity.Patch(targetUsageEntity);
                        }
                        else
                        {
                            changedEntries.Add(new GenericChangedEntry<PromotionUsage>(usage, EntryState.Added));
                            repository.Add(sourceEntity);
                        }
                    }
                }
                await repository.UnitOfWork.CommitAsync();
                pkMap.ResolvePrimaryKeys();
                await _eventPublisher.Publish(new PromotionUsageChangedEvent(changedEntries));
            }

            PromotionUsageCacheRegion.ExpireRegion();
        }

        public virtual async Task DeleteUsagesAsync(string[] ids)
        {
            using (var repository = _repositoryFactory())
            {
                await repository.RemoveMarketingUsagesAsync(ids);
                await repository.UnitOfWork.CommitAsync();
            }

            PromotionUsageCacheRegion.ExpireRegion();
        }

        #endregion

        protected virtual IList<SortInfo> GetPromotionUsageSortInfo(PromotionUsageSearchCriteria criteria)
        {
            var sortInfos = criteria.SortInfos;
            if (sortInfos.IsNullOrEmpty())
            {
                sortInfos = new[] { new SortInfo { SortColumn = ReflectionUtility.GetPropertyName<PromotionUsage>(x => x.ModifiedDate), SortDirection = SortDirection.Descending } };
            }

            return sortInfos;
        }

        protected virtual IQueryable<PromotionUsageEntity> GetPromotionUsageQuery(IMarketingRepository repository, PromotionUsageSearchCriteria criteria, IList<SortInfo> sortInfos)
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
