using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.Platform.Data.Infrastructure;
using VirtoCommerce.SubscriptionModule.Core.Events;
using VirtoCommerce.SubscriptionModule.Core.Model;
using VirtoCommerce.SubscriptionModule.Core.Model.Search;
using VirtoCommerce.SubscriptionModule.Core.Services;
using VirtoCommerce.SubscriptionModule.Data.Caching;
using VirtoCommerce.SubscriptionModule.Data.Model;
using VirtoCommerce.SubscriptionModule.Data.Repositories;

namespace VirtoCommerce.SubscriptionModule.Data.Services
{
    public class PaymentPlanService : IPaymentPlanService, IPaymentPlanSearchService
    {
        public PaymentPlanService(Func<ISubscriptionRepository> subscriptionRepositoryFactory, IEventPublisher eventPublisher,
            IPlatformMemoryCache platformMemoryCache)
        {
            SubscriptionRepositoryFactory = subscriptionRepositoryFactory;
            EventPublisher = eventPublisher;
            PlatformMemoryCache = platformMemoryCache;
        }

        protected IEventPublisher EventPublisher { get; }
        protected Func<ISubscriptionRepository> SubscriptionRepositoryFactory { get; }
        protected IPlatformMemoryCache PlatformMemoryCache { get; }

        #region IPaymentPlanService Members

        public virtual async Task<PaymentPlan[]> GetByIdsAsync(string[] planIds, string responseGroup = null)
        {
            var cacheKey = CacheKey.With(GetType(), nameof(GetByIdsAsync), string.Join("-", planIds), responseGroup);
            return await PlatformMemoryCache.GetOrCreateExclusiveAsync(cacheKey, async cacheEntry =>
            {
                cacheEntry.AddExpirationToken(PaymentPlanCacheRegion.CreateChangeToken());

                var retVal = new List<PaymentPlan>();

                using (var repository = SubscriptionRepositoryFactory())
                {
                    repository.DisableChangesTracking();

                    var paymentPlanEntities = await repository.GetPaymentPlansByIdsAsync(planIds);
                    foreach (var paymentPlanEntity in paymentPlanEntities)
                    {
                        var paymentPlan = AbstractTypeFactory<PaymentPlan>.TryCreateInstance();
                        if (paymentPlan != null)
                        {
                            paymentPlan = paymentPlanEntity.ToModel(paymentPlan);
                            retVal.Add(paymentPlan);

                            cacheEntry.AddExpirationToken(PaymentPlanCacheRegion.CreateChangeToken(paymentPlan));
                        }
                    }
                }
                return retVal.ToArray();
            });
        }

        public virtual async Task SavePlansAsync(PaymentPlan[] plans)
        {
            var pkMap = new PrimaryKeyResolvingMap();
            var changedEntries = new List<GenericChangedEntry<PaymentPlan>>();

            using (var repository = SubscriptionRepositoryFactory())
            {
                var existPlanEntities = await repository.GetPaymentPlansByIdsAsync(plans.Where(x => !x.IsTransient()).Select(x => x.Id).ToArray());
                foreach (var paymentPlan in plans)
                {
                    var sourcePlanEntity = AbstractTypeFactory<PaymentPlanEntity>.TryCreateInstance();
                    if (sourcePlanEntity != null)
                    {
                        sourcePlanEntity = sourcePlanEntity.FromModel(paymentPlan, pkMap) as PaymentPlanEntity;
                        var targetPlanEntity = existPlanEntities.FirstOrDefault(x => x.Id == paymentPlan.Id);
                        if (targetPlanEntity != null)
                        {
                            changedEntries.Add(new GenericChangedEntry<PaymentPlan>(paymentPlan, targetPlanEntity.ToModel(AbstractTypeFactory<PaymentPlan>.TryCreateInstance()), EntryState.Modified));
                            sourcePlanEntity.Patch(targetPlanEntity);
                        }
                        else
                        {
                            repository.Add(sourcePlanEntity);
                            changedEntries.Add(new GenericChangedEntry<PaymentPlan>(paymentPlan, EntryState.Added));
                        }
                    }
                }

                //Raise domain events
                await EventPublisher.Publish(new PaymentPlanChangingEvent(changedEntries));
                await repository.UnitOfWork.CommitAsync();
                pkMap.ResolvePrimaryKeys();
                await EventPublisher.Publish(new PaymentPlanChangedEvent(changedEntries));
            }

            ClearCacheFor(plans);
        }

        public virtual async Task DeleteAsync(string[] ids)
        {
            using (var repository = SubscriptionRepositoryFactory())
            {
                var paymentPlans = await GetByIdsAsync(ids);
                if (!paymentPlans.IsNullOrEmpty())
                {
                    var changedEntries = paymentPlans.Select(x => new GenericChangedEntry<PaymentPlan>(x, EntryState.Deleted));
                    await EventPublisher.Publish(new PaymentPlanChangingEvent(changedEntries));

                    await repository.RemovePaymentPlansByIdsAsync(ids);
                    await repository.UnitOfWork.CommitAsync();

                    await EventPublisher.Publish(new PaymentPlanChangedEvent(changedEntries));
                }

                ClearCacheFor(paymentPlans);
            }
        }

        #endregion

        #region IPaymentPlanSearchService members

        public virtual async Task<GenericSearchResult<PaymentPlan>> SearchPlansAsync(PaymentPlanSearchCriteria criteria)
        {
            var cacheKey = CacheKey.With(GetType(), nameof(SearchPlansAsync), criteria.GetCacheKey());
            return await PlatformMemoryCache.GetOrCreateExclusiveAsync(cacheKey, async cacheEntry =>
            {
                cacheEntry.AddExpirationToken(PaymentPlanSearchCacheRegion.CreateChangeToken());

                var retVal = new GenericSearchResult<PaymentPlan>();
                using (var repository = SubscriptionRepositoryFactory())
                {
                    repository.DisableChangesTracking();

                    var sortInfos = GetSearchSortInfos(criteria);
                    var query = GetSearchQuery(repository, sortInfos);

                    retVal.TotalCount = await query.CountAsync();

                    if (criteria.Take > 0)
                    {
                        var paymentPlanEntities = await query.Skip(criteria.Skip).Take(criteria.Take).ToArrayAsync();
                        var paymentPlanIds = paymentPlanEntities.Select(x => x.Id).ToArray();

                        //Load subscriptions with preserving sorting order
                        var unorderedResults = await GetByIdsAsync(paymentPlanIds, criteria.ResponseGroup);
                        retVal.Results = unorderedResults.AsQueryable().OrderBySortInfos(sortInfos).ToArray();
                    }

                    return retVal;
                }
            });
        }

        #endregion

        protected virtual void ClearCacheFor(PaymentPlan[] paymentPlans)
        {
            foreach (var paymentPlan in paymentPlans)
            {
                PaymentPlanCacheRegion.ExpirePaymentPlan(paymentPlan);
            }

            PaymentPlanSearchCacheRegion.ExpireRegion();
        }

        protected virtual IList<SortInfo> GetSearchSortInfos(PaymentPlanSearchCriteria criteria)
        {
            var sortInfos = criteria.SortInfos;
            if (sortInfos.IsNullOrEmpty())
            {
                sortInfos = new[] { new SortInfo { SortColumn = ReflectionUtility.GetPropertyName<PaymentPlan>(x => x.CreatedDate), SortDirection = SortDirection.Descending } };
            }

            return sortInfos;
        }

        protected virtual IQueryable<PaymentPlanEntity> GetSearchQuery(ISubscriptionRepository repository, IList<SortInfo> sortInfos)
        {
            var query = repository.PaymentPlans;

            query = query.OrderBySortInfos(sortInfos);
            return query;
        }
    }
}
