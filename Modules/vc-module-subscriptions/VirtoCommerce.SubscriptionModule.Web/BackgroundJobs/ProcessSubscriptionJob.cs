using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using VirtoCommerce.SubscriptionModule.Core.Model;
using VirtoCommerce.SubscriptionModule.Core.Model.Search;
using VirtoCommerce.SubscriptionModule.Core.Services;

namespace VirtoCommerce.SubscriptionModule.Web.BackgroundJobs
{
    public class ProcessSubscriptionJob
    {
        private readonly ISubscriptionBuilder _subscriptionBuilder;
        private readonly ISubscriptionSearchService _subscriptionSearchService;
        private readonly ISubscriptionService _subscriptionService;
        public ProcessSubscriptionJob(ISubscriptionBuilder subscriptionBuilder, ISubscriptionSearchService subscriptionSearchService,
                                      ISubscriptionService subscriptionService)
        {
            _subscriptionBuilder = subscriptionBuilder;
            _subscriptionSearchService = subscriptionSearchService;
            _subscriptionService = subscriptionService;
        }

        [DisableConcurrentExecution(60 * 60 * 24)]
        public async Task Process()
        {
            var criteria = new SubscriptionSearchCriteria
            {
                Statuses = new[] { SubscriptionStatus.Active, SubscriptionStatus.PastDue, SubscriptionStatus.Trialing, SubscriptionStatus.Unpaid }.Select(x => x.ToString()).ToArray(),
                Take = 0
            };
            var result = await _subscriptionSearchService.SearchSubscriptionsAsync(criteria);
            var batchSize = 20;
            for (var i = 0; i < result.TotalCount; i += batchSize)
            {
                criteria.Skip = i;
                criteria.Take = batchSize;
                result = await _subscriptionSearchService.SearchSubscriptionsAsync(criteria);
                var subscriptions = await _subscriptionService.GetByIdsAsync(result.Results.Select(x => x.Id).ToArray());
                foreach (var subscription in subscriptions)
                {
                    await _subscriptionBuilder.TakeSubscription(subscription).ActualizeAsync();
                }
                await _subscriptionService.SaveSubscriptionsAsync(subscriptions);
            }
        }

    }
}
