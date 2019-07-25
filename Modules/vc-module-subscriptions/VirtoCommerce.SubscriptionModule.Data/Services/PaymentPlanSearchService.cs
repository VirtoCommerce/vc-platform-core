using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Data.Infrastructure;
using VirtoCommerce.SubscriptionModule.Core.Model.Search;
using VirtoCommerce.SubscriptionModule.Core.Services;
using VirtoCommerce.SubscriptionModule.Data.Caching;
using VirtoCommerce.SubscriptionModule.Data.Model;
using VirtoCommerce.SubscriptionModule.Data.Repositories;

namespace VirtoCommerce.SubscriptionModule.Data.Services
{
    public class PaymentPlanSearchService : IPaymentPlanSearchService
    {
        private readonly Func<ISubscriptionRepository> _subscriptionRepositoryFactory;
        private readonly IPlatformMemoryCache _platformMemoryCache;
        private readonly IPaymentPlanService _paymentPlanService;

        public PaymentPlanSearchService(
            Func<ISubscriptionRepository> subscriptionRepositoryFactory
            , IPlatformMemoryCache platformMemoryCache
            , IPaymentPlanService paymentPlanService
            )
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

                    var query = BuildQuery(repository, criteria);
                    var sortInfos = BuildSortExpression(criteria);

                    retVal.TotalCount = await query.CountAsync();

                    if (criteria.Take > 0)
                    {
                        var paymentPlanIds = await query.OrderBySortInfos(sortInfos).ThenBy(x => x.Id)
                                                             .Select(x=> x.Id)
                                                             .Skip(criteria.Skip).Take(criteria.Take)
                                                             .ToArrayAsync();

                        //Load plans with preserving sorting order
                        var unorderedResults = await _paymentPlanService.GetByIdsAsync(paymentPlanIds, criteria.ResponseGroup);
                        retVal.Results = unorderedResults.OrderBy(x => Array.IndexOf(paymentPlanIds, x.Id)).ToArray();
                    }

                    return retVal;
                }
            });
        }

        protected virtual IQueryable<PaymentPlanEntity> BuildQuery(ISubscriptionRepository repository, PaymentPlanSearchCriteria criteria)
        {
            var query = repository.PaymentPlans;
            return query;
        }

        protected virtual IList<SortInfo> BuildSortExpression(PaymentPlanSearchCriteria criteria)
        {
            var sortInfos = criteria.SortInfos;
            if (sortInfos.IsNullOrEmpty())
            {
                sortInfos = new[]
                {
                    new SortInfo
                    {
                        SortColumn = nameof(PaymentPlanEntity.CreatedDate),
                        SortDirection = SortDirection.Descending
                    }
                };
            }
            return sortInfos;
        }
    }
}
