using System.Linq;
using Hangfire;
using VirtoCommerce.SubscriptionModule.Core.Model;
using VirtoCommerce.SubscriptionModule.Core.Model.Search;
using VirtoCommerce.SubscriptionModule.Core.Services;

namespace VirtoCommerce.SubscriptionModule.Web.BackgroundJobs
{
    public class CreateRecurrentOrdersJob
    {

        private readonly ISubscriptionBuilder _subscriptionBuilder;
        private readonly ISubscriptionSearchService _subscriptionSearchService;
        private readonly ISubscriptionService _subscriptionService;
        private readonly ICustomerOrderService _customerOrderService;

        public CreateRecurrentOrdersJob(ISubscriptionBuilder subscriptionBuilder, ISubscriptionSearchService subscriptionSearchService,
            ISubscriptionService subscriptionService, ICustomerOrderService customerOrderService)
        {
            _subscriptionBuilder = subscriptionBuilder;
            _subscriptionSearchService = subscriptionSearchService;
            _subscriptionService = subscriptionService;
            _customerOrderService = customerOrderService;
        }

        [DisableConcurrentExecution(60 * 60 * 24)]
        public void Process()
        {
            var criteria = new SubscriptionSearchCriteria
            {
                Statuses = new[] { SubscriptionStatus.Active, SubscriptionStatus.PastDue, SubscriptionStatus.Trialing, SubscriptionStatus.Unpaid }.Select(x => x.ToString()).ToArray(),
                Take = 0,
            };
            var result = _subscriptionSearchService.SearchSubscriptions(criteria);
            var batchSize = 20;
            for (var i = 0; i < result.TotalCount; i += batchSize)
            {
                criteria.Skip = i;
                criteria.Take = batchSize;
                result = _subscriptionSearchService.SearchSubscriptions(criteria);
                var subscriptions = _subscriptionService.GetByIds(result.Results.Select(x => x.Id).ToArray());
                foreach (var subscription in subscriptions)
                {
                    var newOrder = _subscriptionBuilder.TakeSubscription(subscription).Actualize().TryToCreateRecurrentOrder();
                    if (newOrder != null)
                    {
                        _customerOrderService.SaveChanges(new[] { newOrder });
                    }
                }
            }
        }
    }
}
