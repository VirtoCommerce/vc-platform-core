using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using VirtoCommerce.OrdersModule.Core.Model;
using VirtoCommerce.OrdersModule.Core.Model.Search;
using VirtoCommerce.OrdersModule.Core.Services;
using VirtoCommerce.OrdersModule.Data.Caching;
using VirtoCommerce.OrdersModule.Data.Model;
using VirtoCommerce.OrdersModule.Data.Repositories;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Data.Infrastructure;

namespace VirtoCommerce.OrdersModule.Data.Services
{
    public class CustomerOrderSearchService : ICustomerOrderSearchService
    {
        private readonly Func<IOrderRepository> _repositoryFactory;
        private readonly IPlatformMemoryCache _platformMemoryCache;
        private readonly ICustomerOrderService _customerOrderService;

        public CustomerOrderSearchService(Func<IOrderRepository> repositoryFactory, ICustomerOrderService customerOrderService, IPlatformMemoryCache platformMemoryCache)
        {
            _repositoryFactory = repositoryFactory;
            _platformMemoryCache = platformMemoryCache;
            _customerOrderService = customerOrderService;
        }

        public virtual async Task<CustomerOrderSearchResult> SearchCustomerOrdersAsync(CustomerOrderSearchCriteria criteria)
        {
            var cacheKey = CacheKey.With(GetType(), nameof(SearchCustomerOrdersAsync), criteria.GetCacheKey());
            return await _platformMemoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                cacheEntry.AddExpirationToken(OrderSearchCacheRegion.CreateChangeToken());
                using (var repository = _repositoryFactory())
                {
                    repository.DisableChangesTracking();
                    var result = AbstractTypeFactory<CustomerOrderSearchResult>.TryCreateInstance();
                    var orderResponseGroup = EnumUtility.SafeParseFlags(criteria.ResponseGroup, CustomerOrderResponseGroup.Full);

                    var query = BuildQuery(repository, criteria);
                    var sortInfos = BuildSortExpression(criteria);

                    result.TotalCount = await query.CountAsync();
                    if (criteria.Take > 0)
                    {
                        var orderIds = await query.OrderBySortInfos(sortInfos).ThenBy(x => x.Id)
                                                         .Select(x => x.Id)
                                                         .Skip(criteria.Skip).Take(criteria.Take)
                                                         .ToArrayAsync();
                        var unorderedResults = await _customerOrderService.GetByIdsAsync(orderIds, criteria.ResponseGroup);
                        result.Results = unorderedResults.OrderBy(x => Array.IndexOf(orderIds, x.Id)).ToList();
                    }                  
                    return result;
                }
            });
        }

        protected virtual IQueryable<CustomerOrderEntity> BuildQuery(IOrderRepository repository, CustomerOrderSearchCriteria criteria)
        {
            var query = repository.CustomerOrders;

            // Don't return prototypes by default
            if (!criteria.WithPrototypes)
            {
                query = query.Where(x => !x.IsPrototype);
            }

            if (!criteria.Ids.IsNullOrEmpty())
            {
                query = query.Where(x => criteria.Ids.Contains(x.Id));
            }

            if (criteria.OnlyRecurring)
            {
                query = query.Where(x => x.SubscriptionId != null);
            }

            if (!criteria.CustomerIds.IsNullOrEmpty())
            {
                query = query.Where(x => criteria.CustomerIds.Contains(x.CustomerId));
            }

            if (criteria.EmployeeId != null)
            {
                query = query.Where(x => x.EmployeeId == criteria.EmployeeId);
            }

            if (criteria.StartDate != null)
            {
                query = query.Where(x => x.CreatedDate >= criteria.StartDate);
            }

            if (criteria.EndDate != null)
            {
                query = query.Where(x => x.CreatedDate <= criteria.EndDate);
            }

            if (!criteria.SubscriptionIds.IsNullOrEmpty())
            {
                query = query.Where(x => criteria.SubscriptionIds.Contains(x.SubscriptionId));
            }

            if (!criteria.Statuses.IsNullOrEmpty())
            {
                query = query.Where(x => criteria.Statuses.Contains(x.Status));
            }

            if (!criteria.StoreIds.IsNullOrEmpty())
            {
                query = query.Where(x => criteria.StoreIds.Contains(x.StoreId));
            }

            if (!criteria.Numbers.IsNullOrEmpty())
            {
                query = query.Where(x => criteria.Numbers.Contains(x.Number));
            }
            else if (!string.IsNullOrEmpty(criteria.Keyword))
            {
                query = query.Where(GetKeywordPredicate(criteria));
            }

            return query;
        }

        protected virtual IList<SortInfo> BuildSortExpression(CustomerOrderSearchCriteria criteria)
        {
            var sortInfos = criteria.SortInfos;
            if (sortInfos.IsNullOrEmpty())
            {
                sortInfos = new[]
                {
                    new SortInfo
                    {
                        SortColumn = nameof(CustomerOrderEntity.CreatedDate),
                        SortDirection = SortDirection.Descending
                    }
                };
            }
            return sortInfos;
        }

        protected virtual Expression<Func<CustomerOrderEntity, bool>> GetKeywordPredicate(CustomerOrderSearchCriteria criteria)
        {
            return order => order.Number.Contains(criteria.Keyword) || order.CustomerName.Contains(criteria.Keyword);
        }
    }
}
