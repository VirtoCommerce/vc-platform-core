using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.OrdersModule.Core.Model;
using VirtoCommerce.OrdersModule.Core.Model.Search;
using VirtoCommerce.OrdersModule.Core.Services;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.SubscriptionModule.Core.Model;
using VirtoCommerce.SubscriptionModule.Core.Model.Search;
using VirtoCommerce.SubscriptionModule.Core.Services;
using VirtoCommerce.SubscriptionModule.Data.Model;
using VirtoCommerce.SubscriptionModule.Data.Repositories;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.Platform.Data.Infrastructure;
using VirtoCommerce.Platform.Core.ChangeLog;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.SubscriptionModule.Core.Events;
using VirtoCommerce.StoreModule.Core.Services;
using VirtoCommerce.SubscriptionModule.Data.Caching;

namespace VirtoCommerce.SubscriptionModule.Data.Services
{
    public class SubscriptionServiceImpl : ISubscriptionService, ISubscriptionSearchService
    {
        private readonly IStoreService _storeService;
        private readonly ICustomerOrderService _customerOrderService;
        private readonly ICustomerOrderSearchService _customerOrderSearchService;
        private readonly Func<ISubscriptionRepository> _subscriptionRepositoryFactory;
        private readonly IUniqueNumberGenerator _uniqueNumberGenerator;
        private readonly IChangeLogService _changeLogService;
        private readonly IEventPublisher _eventPublisher;
        private readonly IPlatformMemoryCache _platformMemoryCache;

        public SubscriptionServiceImpl(Func<ISubscriptionRepository> subscriptionRepositoryFactory, ICustomerOrderService customerOrderService,
                                       ICustomerOrderSearchService customerOrderSearchService, IStoreService storeService,
                                       IUniqueNumberGenerator uniqueNumberGenerator, IChangeLogService changeLogService, IEventPublisher eventPublisher,
                                       IPlatformMemoryCache platformMemoryCache)
        {
            _customerOrderSearchService = customerOrderSearchService;
            _subscriptionRepositoryFactory = subscriptionRepositoryFactory;
            _customerOrderService = customerOrderService;
            _storeService = storeService;
            _uniqueNumberGenerator = uniqueNumberGenerator;
            _changeLogService = changeLogService;
            _eventPublisher = eventPublisher;
            _platformMemoryCache = platformMemoryCache;
        }

        #region ISubscriptionService members

        public async Task<Subscription[]> GetByIdsAsync(string[] subscriptionIds, string responseGroup = null)
        {
            var cacheKey = CacheKey.With(GetType(), nameof(GetByIdsAsync), string.Join("-", subscriptionIds), responseGroup);
            return await _platformMemoryCache.GetOrCreateExclusiveAsync(cacheKey, async cacheEntry =>
            {
                var retVal = new List<Subscription>();
                var subscriptionResponseGroup = EnumUtility.SafeParse(responseGroup, SubscriptionResponseGroup.Full);
                using (var repository = _subscriptionRepositoryFactory())
                {
                    repository.DisableChangesTracking();

                    var subscriptionEntities = await repository.GetSubscriptionsByIdsAsync(subscriptionIds, responseGroup);
                    foreach (var subscriptionEntity in subscriptionEntities)
                    {
                        var subscription = AbstractTypeFactory<Subscription>.TryCreateInstance();
                        if (subscription != null)
                        {
                            subscription = subscriptionEntity.ToModel(subscription) as Subscription;
                            if (subscriptionResponseGroup.HasFlag(SubscriptionResponseGroup.WithChangeLog))
                            {
                                //Load change log by separate request
                                _changeLogService.LoadChangeLogs(subscription);
                            }
                            retVal.Add(subscription);
                        }
                    }
                }

                CustomerOrder[] orderPrototypes = null;
                CustomerOrder[] subscriptionOrders = null;

                if (subscriptionResponseGroup.HasFlag(SubscriptionResponseGroup.WithOrderPrototype))
                {
                    orderPrototypes = await _customerOrderService.GetByIdsAsync(retVal.Select(x => x.CustomerOrderPrototypeId).ToArray());
                }
                if (subscriptionResponseGroup.HasFlag(SubscriptionResponseGroup.WithRelatedOrders))
                {
                    //Loads customer order prototypes and related orders for each subscription via order service
                    var criteria = new CustomerOrderSearchCriteria
                    {
                        SubscriptionIds = subscriptionIds
                    };
                    subscriptionOrders = (await _customerOrderSearchService.SearchCustomerOrdersAsync(criteria)).Results.ToArray();
                }

                foreach (var subscription in retVal)
                {
                    if (!orderPrototypes.IsNullOrEmpty())
                    {
                        subscription.CustomerOrderPrototype = orderPrototypes.FirstOrDefault(x => x.Id == subscription.CustomerOrderPrototypeId);
                    }
                    if (!subscriptionOrders.IsNullOrEmpty())
                    {
                        subscription.CustomerOrders = subscriptionOrders.Where(x => x.SubscriptionId == subscription.Id).ToList();
                        subscription.CustomerOrdersIds = subscription.CustomerOrders.Select(x => x.Id).ToArray();
                    }

                    cacheEntry.AddExpirationToken(SubscriptionCacheRegion.CreateChangeToken(subscription));
                }

                return retVal.ToArray();
            });
        }

        public async Task SaveSubscriptionsAsync(Subscription[] subscriptions)
        {
            var pkMap = new PrimaryKeyResolvingMap();
            var changedEntries = new List<GenericChangedEntry<Subscription>>();

            using (var repository = _subscriptionRepositoryFactory())
            {
                var existEntities = await repository.GetSubscriptionsByIdsAsync(subscriptions.Where(x => !x.IsTransient()).Select(x => x.Id).ToArray());
                foreach (var subscription in subscriptions)
                {
                    //Generate numbers for new subscriptions
                    if (string.IsNullOrEmpty(subscription.Number))
                    {
                        var store = await _storeService.GetByIdAsync(subscription.StoreId);
                        var numberTemplate = store.Settings.GetSettingValue("Subscription.SubscriptionNewNumberTemplate", "SU{0:yyMMdd}-{1:D5}");
                        subscription.Number = _uniqueNumberGenerator.GenerateNumber(numberTemplate);
                    }
                    //Save subscription order prototype with same as subscription Number
                    if (subscription.CustomerOrderPrototype != null)
                    {
                        subscription.CustomerOrderPrototype.Number = subscription.Number;
                        subscription.CustomerOrderPrototype.IsPrototype = true;
                        await _customerOrderService.SaveChangesAsync(new[] { subscription.CustomerOrderPrototype });
                    }
                    var originalEntity = existEntities.FirstOrDefault(x => x.Id == subscription.Id);
                    var originalSubscription = originalEntity != null ? originalEntity.ToModel(AbstractTypeFactory<Subscription>.TryCreateInstance()) : subscription;

                    var modifiedEntity = AbstractTypeFactory<SubscriptionEntity>.TryCreateInstance().FromModel(subscription, pkMap);
                    if (originalEntity != null)
                    {
                        changedEntries.Add(new GenericChangedEntry<Subscription>(subscription, originalEntity.ToModel(AbstractTypeFactory<Subscription>.TryCreateInstance()), EntryState.Modified));
                        modifiedEntity.Patch(originalEntity);
                        //force the subscription.ModifiedDate update, because the subscription object may not have any changes in its properties
                        originalEntity.ModifiedDate = DateTime.UtcNow;
                    }
                    else
                    {
                        repository.Add(modifiedEntity);
                        changedEntries.Add(new GenericChangedEntry<Subscription>(subscription, EntryState.Added));
                    }
                }

                //Raise domain events
                await _eventPublisher.Publish(new SubscriptionChangingEvent(changedEntries));
                await repository.UnitOfWork.CommitAsync();
                pkMap.ResolvePrimaryKeys();
                await _eventPublisher.Publish(new SubscriptionChangedEvent(changedEntries));
            }

            ClearCacheFor(subscriptions);
        }

        public async Task DeleteAsync(string[] ids)
        {
            using (var repository = _subscriptionRepositoryFactory())
            {
                var subscriptions = await GetByIdsAsync(ids);
                if (!subscriptions.IsNullOrEmpty())
                {
                    var changedEntries = subscriptions.Select(x => new GenericChangedEntry<Subscription>(x, EntryState.Deleted));
                    await _eventPublisher.Publish(new SubscriptionChangingEvent(changedEntries));

                    //Remove subscription order prototypes
                    var orderPrototypesIds = repository.Subscriptions.Where(x => ids.Contains(x.Id)).Select(x => x.CustomerOrderPrototypeId).ToArray();
                    await _customerOrderService.DeleteAsync(orderPrototypesIds);

                    await repository.RemoveSubscriptionsByIdsAsync(ids);
                    await repository.UnitOfWork.CommitAsync();

                    await _eventPublisher.Publish(new SubscriptionChangedEvent(changedEntries));

                    ClearCacheFor(subscriptions);
                }
            }
        }
        #endregion

        #region ISubscriptionSearchService members
        public async Task<GenericSearchResult<Subscription>> SearchSubscriptionsAsync(SubscriptionSearchCriteria criteria)
        {
            var cacheKey = CacheKey.With(GetType(), nameof(SearchSubscriptionsAsync), criteria.GetCacheKey());
            return await _platformMemoryCache.GetOrCreateExclusiveAsync(cacheKey, async cacheEntry =>
            {
                cacheEntry.AddExpirationToken(SubscriptionSearchCacheRegion.CreateChangeToken());

                var retVal = new GenericSearchResult<Subscription>();
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
                        var unorderedResults = await GetByIdsAsync(subscriptionsIds, criteria.ResponseGroup);
                        retVal.Results = unorderedResults.OrderBy(x => Array.IndexOf(subscriptionsIds, x.Id)).ToArray();
                    }

                    return retVal;
                }
            });
        }
        #endregion

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

        protected virtual void ClearCacheFor(Subscription[] subscriptions)
        {
            foreach (var subscription in subscriptions)
            {
                SubscriptionCacheRegion.ExpireSubscription(subscription);
            }

            SubscriptionSearchCacheRegion.ExpireRegion();
        }
    }
}
