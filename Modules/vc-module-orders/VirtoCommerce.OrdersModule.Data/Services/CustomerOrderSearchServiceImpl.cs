using System;
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
    public class CustomerOrderSearchServiceImpl : ICustomerOrderSearchService
    {
        public CustomerOrderSearchServiceImpl(Func<IOrderRepository> repositoryFactory, IPlatformMemoryCache platformMemoryCache)
        {
            RepositoryFactory = repositoryFactory;
            PlatformMemoryCache = platformMemoryCache;
        }

        protected Func<IOrderRepository> RepositoryFactory { get; }
        protected IPlatformMemoryCache PlatformMemoryCache { get; }

        public virtual async Task<GenericSearchResult<CustomerOrder>> SearchCustomerOrdersAsync(CustomerOrderSearchCriteria criteria)
        {
            var cacheKey = CacheKey.With(GetType(), "SearchCustomerOrdersAsync", criteria.GetCacheKey());
            return await PlatformMemoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                cacheEntry.AddExpirationToken(OrderSearchCacheRegion.CreateChangeToken());
                using (var repository = RepositoryFactory())
                {
                    repository.DisableChangesTracking();
                    var retVal = new GenericSearchResult<CustomerOrder>();
                    var orderResponseGroup = EnumUtility.SafeParse(criteria.ResponseGroup, CustomerOrderResponseGroup.Full);

                    var query = GetOrdersQuery(repository, criteria);

                    var sortInfos = criteria.SortInfos;
                    if (sortInfos.IsNullOrEmpty())
                    {
                        sortInfos = new[]
                        {
                            new SortInfo
                            {
                                SortColumn = ReflectionUtility.GetPropertyName<CustomerOrderEntity>(x => x.CreatedDate),
                                SortDirection = SortDirection.Descending
                            }
                        };
                    }
                    query = query.OrderBySortInfos(sortInfos);

                    retVal.TotalCount = await query.CountAsync();
                    var orderIds = await query.Select(x => x.Id).Skip(criteria.Skip).Take(criteria.Take).ToArrayAsync();

                    var list = await repository.GetCustomerOrdersByIdsAsync(orderIds, orderResponseGroup);

                    retVal.Results = list.Select(x =>
                        x.ToModel(AbstractTypeFactory<CustomerOrder>.TryCreateInstance()) as CustomerOrder).ToList();

                    return retVal;
                }
            });
        }

        protected virtual IQueryable<CustomerOrderEntity> GetOrdersQuery(IOrderRepository repository, CustomerOrderSearchCriteria criteria)
        {
            var query = repository.CustomerOrders;

            // Don't return prototypes by default
            if (!criteria.WithPrototypes)
            {
                query = query.Where(x => !x.IsPrototype);
            }

            if (criteria.OnlyRecurring)
            {
                query = query.Where(x => x.SubscriptionId != null);
            }

            if (criteria.CustomerId != null)
            {
                query = query.Where(x => x.CustomerId == criteria.CustomerId);
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

            if (criteria.Statuses != null && criteria.Statuses.Any())
            {
                query = query.Where(x => criteria.Statuses.Contains(x.Status));
            }

            if (criteria.StoreIds != null && criteria.StoreIds.Any())
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

        protected virtual Expression<Func<CustomerOrderEntity, bool>> GetKeywordPredicate(CustomerOrderSearchCriteria criteria)
        {
            return order => order.Number.Contains(criteria.Keyword) || order.CustomerName.Contains(criteria.Keyword);
        }
    }
}
