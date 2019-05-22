using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using VirtoCommerce.OrdersModule.Core.Services;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Data.Infrastructure;
using VirtoCommerce.SubscriptionModule.Core.Model;
using VirtoCommerce.SubscriptionModule.Core.Model.Search;
using VirtoCommerce.SubscriptionModule.Core.Services;
using VirtoCommerce.SubscriptionModule.Data.Caching;
using VirtoCommerce.SubscriptionModule.Data.Model;
using VirtoCommerce.SubscriptionModule.Data.Repositories;

namespace VirtoCommerce.SubscriptionModule.Data.Services
{
    public class SubscriptionSearchService : ISubscriptionSearchService
    {
        private readonly Func<ISubscriptionRepository> _subscriptionRepositoryFactory;
        private readonly IPlatformMemoryCache _platformMemoryCache;
        private readonly ISubscriptionService _subscriptionService;
        private readonly ICustomerOrderService _customerOrderService;

        public SubscriptionSearchService(Func<ISubscriptionRepository> subscriptionRepositoryFactory, IPlatformMemoryCache platformMemoryCache, ISubscriptionService subscriptionService, ICustomerOrderService customerOrderService)
        {
            _subscriptionRepositoryFactory = subscriptionRepositoryFactory;
            _platformMemoryCache = platformMemoryCache;
            _subscriptionService = subscriptionService;
            _customerOrderService = customerOrderService;
        }

        public virtual async Task<SubscriptionSearchResult> SearchSubscriptionsAsync(SubscriptionSearchCriteria criteria)
        {
            var cacheKey = CacheKey.With(GetType(), nameof(SearchSubscriptionsAsync), criteria.GetCacheKey());
            return await _platformMemoryCache.GetOrCreateExclusiveAsync(cacheKey, async cacheEntry =>
            {
                cacheEntry.AddExpirationToken(SubscriptionSearchCacheRegion.CreateChangeToken());

                var retVal = AbstractTypeFactory<SubscriptionSearchResult>.TryCreateInstance();
                using (var repository = _subscriptionRepositoryFactory())
                {
                    repository.DisableChangesTracking();

                    var query = await GetSubscriptionsQueryForCriteria(repository, criteria);

                    retVal.TotalCount = await query.CountAsync();

                    if (criteria.Take > 0)
                    {
                        var subscriptionEntities = await query.Skip(criteria.Skip).Take(criteria.Take).ToArrayAsync();
                        var subscriptionsIds = subscriptionEntities.Select(x => x.Id).ToArray();

                        //Load subscriptions with preserving sorting order
                        var unorderedResults = await _subscriptionService.GetByIdsAsync(subscriptionsIds, criteria.ResponseGroup);
                        retVal.Results = unorderedResults.OrderBy(x => Array.IndexOf(subscriptionsIds, x.Id)).ToArray();
                    }

                    return retVal;
                }
            });
        }

        protected virtual async Task<IQueryable<SubscriptionEntity>> GetSubscriptionsQueryForCriteria(
            ISubscriptionRepository repository, SubscriptionSearchCriteria criteria)
        {
            var query = repository.Subscriptions;

            if (!string.IsNullOrEmpty(criteria.Number))
            {
                query = query.Where(x => x.Number == criteria.Number);
            }
            else if (criteria.Keyword != null)
            {
                query = query.Where(x => x.Number.Contains(criteria.Keyword));
            }

            if (criteria.CustomerId != null)
            {
                query = query.Where(x => x.CustomerId == criteria.CustomerId);
            }
            if (criteria.Statuses != null && criteria.Statuses.Any())
            {
                query = query.Where(x => criteria.Statuses.Contains(x.Status));
            }
            if (criteria.StoreId != null)
            {
                query = query.Where(x => criteria.StoreId == x.StoreId);
            }

            if (criteria.StartDate != null)
            {
                query = query.Where(x => x.CreatedDate >= criteria.StartDate);
            }

            if (criteria.EndDate != null)
            {
                query = query.Where(x => x.CreatedDate <= criteria.EndDate);
            }

            if (criteria.ModifiedSinceDate != null)
            {
                query = query.Where(x => x.ModifiedDate >= criteria.ModifiedSinceDate);
            }

            if (!string.IsNullOrEmpty(criteria.CustomerOrderId))
            {
                var order = (await _customerOrderService.GetByIdsAsync(new[] { criteria.CustomerOrderId })).FirstOrDefault();
                if (order != null && !string.IsNullOrEmpty(order.SubscriptionId))
                {
                    query = query.Where(x => x.Id == order.SubscriptionId);
                }
                else
                {
                    query = query.Where(x => false);
                }
            }

            if (criteria.OuterId != null)
            {
                query = query.Where(x => x.OuterId == criteria.OuterId);
            }

            var sortInfos = criteria.SortInfos;
            if (sortInfos.IsNullOrEmpty())
            {
                sortInfos = new[] { new SortInfo { SortColumn = ReflectionUtility.GetPropertyName<Subscription>(x => x.CreatedDate), SortDirection = SortDirection.Descending } };
            }
            query = query.OrderBySortInfos(sortInfos);

            return query;
        }
    }
}
