using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.Platform.Data.Infrastructure;
using VirtoCommerce.SubscriptionModule.Core.Events;
using VirtoCommerce.SubscriptionModule.Core.Model;
using VirtoCommerce.SubscriptionModule.Core.Model.Search;
using VirtoCommerce.SubscriptionModule.Core.Services;
using VirtoCommerce.SubscriptionModule.Data.Model;
using VirtoCommerce.SubscriptionModule.Data.Repositories;

namespace VirtoCommerce.SubscriptionModule.Data.Services
{
    public class PaymentPlanService : IPaymentPlanService, IPaymentPlanSearchService
    {
        private readonly IEventPublisher _eventPublisher;
        private readonly Func<ISubscriptionRepository> _subscriptionRepositoryFactory;
        public PaymentPlanService(Func<ISubscriptionRepository> subscriptionRepositoryFactory, IEventPublisher eventPublisher)
        {
            _subscriptionRepositoryFactory = subscriptionRepositoryFactory;
            _eventPublisher = eventPublisher;
        }

        #region IPaymentPlanService Members

        public Task<PaymentPlan[]> GetByIdsAsync(string[] planIds, string responseGroup = null)
        {
            var retVal = new List<PaymentPlan>();

            using (var repository = _subscriptionRepositoryFactory())
            {
                repository.DisableChangesTracking();

                retVal = repository.GetPaymentPlansByIds(planIds).Select(x => x.ToModel(AbstractTypeFactory<PaymentPlan>.TryCreateInstance())).ToList();
            }
            return Task.FromResult(retVal.ToArray());
        }

        public async Task SavePlansAsync(PaymentPlan[] plans)
        {
            var pkMap = new PrimaryKeyResolvingMap();
            var changedEntries = new List<GenericChangedEntry<PaymentPlan>>();

            using (var repository = _subscriptionRepositoryFactory())
            {
                var existPlanEntities = repository.GetPaymentPlansByIds(plans.Where(x => !x.IsTransient()).Select(x => x.Id).ToArray());
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

            // TODO: cache management
        }

        public async Task DeleteAsync(string[] ids)
        {
            using (var repository = _subscriptionRepositoryFactory())
            {
                var paymentPlans = await GetByIdsAsync(ids);
                if (!paymentPlans.IsNullOrEmpty())
                {
                    var changedEntries = paymentPlans.Select(x => new GenericChangedEntry<PaymentPlan>(x, EntryState.Deleted));
                    await _eventPublisher.Publish(new PaymentPlanChangingEvent(changedEntries));

                    repository.RemovePaymentPlansByIds(ids);
                    await repository.UnitOfWork.CommitAsync();

                    await _eventPublisher.Publish(new PaymentPlanChangedEvent(changedEntries));
                }
            }
        }

        #endregion


        #region IPaymentPlanSearchService members
        public async Task<GenericSearchResult<PaymentPlan>> SearchPlansAsync(PaymentPlanSearchCriteria criteria)
        {
            var retVal = new GenericSearchResult<PaymentPlan>();
            using (var repository = _subscriptionRepositoryFactory())
            {
                repository.DisableChangesTracking();

                var query = repository.PaymentPlans;

                var sortInfos = criteria.SortInfos;
                if (sortInfos.IsNullOrEmpty())
                {
                    sortInfos = new[] { new SortInfo { SortColumn = ReflectionUtility.GetPropertyName<PaymentPlan>(x => x.CreatedDate), SortDirection = SortDirection.Descending } };
                }
                query = query.OrderBySortInfos(sortInfos);

                retVal.TotalCount = query.Count();

                var paymentPlanIds = query.Skip(criteria.Skip)
                                            .Take(criteria.Take)
                                            .ToArray()
                                            .Select(x => x.Id)
                                            .ToArray();

                //Load subscriptions with preserving sorting order
                var unorderedResults = await GetByIdsAsync(paymentPlanIds, criteria.ResponseGroup);
                retVal.Results = unorderedResults.OrderBy(x => Array.IndexOf(paymentPlanIds, x.Id)).ToArray();
                return retVal;
            }
        }
        #endregion

    }
}
