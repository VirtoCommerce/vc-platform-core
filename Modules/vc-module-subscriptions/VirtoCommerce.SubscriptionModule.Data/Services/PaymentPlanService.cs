using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.Platform.Data.Infrastructure;
using VirtoCommerce.SubscriptionModule.Core.Events;
using VirtoCommerce.SubscriptionModule.Core.Model;
using VirtoCommerce.SubscriptionModule.Core.Services;
using VirtoCommerce.SubscriptionModule.Data.Caching;
using VirtoCommerce.SubscriptionModule.Data.Model;
using VirtoCommerce.SubscriptionModule.Data.Repositories;

namespace VirtoCommerce.SubscriptionModule.Data.Services
{
    public class PaymentPlanService : IPaymentPlanService
    {
        private readonly IEventPublisher _eventPublisher;
        private readonly Func<ISubscriptionRepository> _subscriptionRepositoryFactory;
        private readonly IPlatformMemoryCache _platformMemoryCache;

        public PaymentPlanService(Func<ISubscriptionRepository> subscriptionRepositoryFactory, IEventPublisher eventPublisher,
            IPlatformMemoryCache platformMemoryCache)
        {
            _subscriptionRepositoryFactory = subscriptionRepositoryFactory;
            _eventPublisher = eventPublisher;
            _platformMemoryCache = platformMemoryCache;
        }


        #region IPaymentPlanService Members

        public virtual async Task<PaymentPlan[]> GetByIdsAsync(string[] planIds, string responseGroup = null)
        {
            var cacheKey = CacheKey.With(GetType(), nameof(GetByIdsAsync), string.Join("-", planIds), responseGroup);
            return await _platformMemoryCache.GetOrCreateExclusiveAsync(cacheKey, async cacheEntry =>
            {
                cacheEntry.AddExpirationToken(PaymentPlanCacheRegion.CreateChangeToken());

                var retVal = new List<PaymentPlan>();

                using (var repository = _subscriptionRepositoryFactory())
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

            using (var repository = _subscriptionRepositoryFactory())
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
                await _eventPublisher.Publish(new PaymentPlanChangingEvent(changedEntries));
                await repository.UnitOfWork.CommitAsync();
                pkMap.ResolvePrimaryKeys();
                await _eventPublisher.Publish(new PaymentPlanChangedEvent(changedEntries));
            }

            ClearCacheFor(plans);
        }

        public virtual async Task DeleteAsync(string[] ids)
        {
            using (var repository = _subscriptionRepositoryFactory())
            {
                var paymentPlans = await GetByIdsAsync(ids);
                if (!paymentPlans.IsNullOrEmpty())
                {
                    var changedEntries = paymentPlans.Select(x => new GenericChangedEntry<PaymentPlan>(x, EntryState.Deleted));
                    await _eventPublisher.Publish(new PaymentPlanChangingEvent(changedEntries));

                    await repository.RemovePaymentPlansByIdsAsync(ids);
                    await repository.UnitOfWork.CommitAsync();

                    await _eventPublisher.Publish(new PaymentPlanChangedEvent(changedEntries));
                }

                ClearCacheFor(paymentPlans);
            }
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
    }
}
