using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Data.Infrastructure;
using VirtoCommerce.SubscriptionModule.Core.Model;
using VirtoCommerce.SubscriptionModule.Core.Model.Search;
using VirtoCommerce.SubscriptionModule.Core.Services;
using VirtoCommerce.SubscriptionModule.Data.Caching;
using VirtoCommerce.SubscriptionModule.Data.Repositories;

namespace VirtoCommerce.SubscriptionModule.Data.Services
{
    public class PaymentPlanSearchService : IPaymentPlanSearchService
    {
        private readonly Func<ISubscriptionRepository> _subscriptionRepositoryFactory;
        private readonly IPlatformMemoryCache _platformMemoryCache;
        private readonly IPaymentPlanService _paymentPlanService;

        public PaymentPlanSearchService(Func<ISubscriptionRepository> subscriptionRepositoryFactory, IPlatformMemoryCache platformMemoryCache, IPaymentPlanService paymentPlanService)
        {
            _subscriptionRepositoryFactory = subscriptionRepositoryFactory;
            _platformMemoryCache = platformMemoryCache;
            _paymentPlanService = paymentPlanService;
        }

        public virtual async Task<PaymentPlanSearchResult> SearchPlansAsync(PaymentPlanSearchCriteria criteria)
        {
            var cacheKey = CacheKey.With(GetType(), nameof(SearchPlansAsync), criteria.GetCacheKey());
            return await _platformMemoryCache.GetOrCreateExclusiveAsync(cacheKey, async cacheEntry =>
            {
                cacheEntry.AddExpirationToken(PaymentPlanSearchCacheRegion.CreateChangeToken());

                var retVal = AbstractTypeFactory<PaymentPlanSearchResult>.TryCreateInstance();
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

                    retVal.TotalCount = await query.CountAsync();

                    if (criteria.Take > 0)
                    {
                        var paymentPlanEntities = await query.Skip(criteria.Skip).Take(criteria.Take).ToArrayAsync();
                        var paymentPlanIds = paymentPlanEntities.Select(x => x.Id).ToArray();

                        //Load subscriptions with preserving sorting order
                        var unorderedResults = await _paymentPlanService.GetByIdsAsync(paymentPlanIds, criteria.ResponseGroup);
                        retVal.Results = unorderedResults.AsQueryable().OrderBySortInfos(sortInfos).ToArray();
                    }

                    return retVal;
                }
            });
        }
    }
}
