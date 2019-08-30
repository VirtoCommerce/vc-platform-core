using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.OrdersModule.Core.Events;
using VirtoCommerce.OrdersModule.Core.Model;
using VirtoCommerce.OrdersModule.Core.Services;
using VirtoCommerce.OrdersModule.Data.Caching;
using VirtoCommerce.OrdersModule.Data.Model;
using VirtoCommerce.OrdersModule.Data.Repositories;
using VirtoCommerce.PaymentModule.Core.Model.Search;
using VirtoCommerce.PaymentModule.Core.Services;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.ChangeLog;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.Platform.Data.Infrastructure;
using VirtoCommerce.ShippingModule.Core.Model.Search;
using VirtoCommerce.ShippingModule.Core.Services;
using VirtoCommerce.StoreModule.Core.Model;
using VirtoCommerce.StoreModule.Core.Services;

namespace VirtoCommerce.OrdersModule.Data.Services
{
    public class CustomerOrderService : ICustomerOrderService
    {
        private readonly Func<IOrderRepository> _repositoryFactory;
        private readonly IEventPublisher _eventPublisher;
        private readonly IStoreService _storeService;

        private readonly IUniqueNumberGenerator _uniqueNumberGenerator;
        private readonly IShippingMethodsSearchService _shippingMethodsSearchService;
        private readonly IPaymentMethodsSearchService _paymentMethodSearchService;
        private readonly IChangeLogService _changeLogService;
        private readonly ICustomerOrderTotalsCalculator _totalsCalculator;
        private readonly IPlatformMemoryCache _platformMemoryCache;

        public CustomerOrderService(
            Func<IOrderRepository> orderRepositoryFactory, IUniqueNumberGenerator uniqueNumberGenerator
            , IStoreService storeService, IChangeLogService changeLogService
            , IEventPublisher eventPublisher, ICustomerOrderTotalsCalculator totalsCalculator
            , IShippingMethodsSearchService shippingMethodsSearchService, IPaymentMethodsSearchService paymentMethodSearchService,
            IPlatformMemoryCache platformMemoryCache)
        {
            _repositoryFactory = orderRepositoryFactory;
            _eventPublisher = eventPublisher;
            _storeService = storeService;
            _changeLogService = changeLogService;
            _totalsCalculator = totalsCalculator;
            _shippingMethodsSearchService = shippingMethodsSearchService;

            _paymentMethodSearchService = paymentMethodSearchService;
            _platformMemoryCache = platformMemoryCache;
            _uniqueNumberGenerator = uniqueNumberGenerator;
        }

        #region ICustomerOrderService Members

        public virtual async Task<CustomerOrder[]> GetByIdsAsync(string[] orderIds, string responseGroup = null)
        {
            var cacheKey = CacheKey.With(GetType(), "GetByIdsAsync", string.Join("-", orderIds), responseGroup);
            return await _platformMemoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                var retVal = new List<CustomerOrder>();
                var orderResponseGroup = EnumUtility.SafeParseFlags(responseGroup, CustomerOrderResponseGroup.Full);

                using (var repository = _repositoryFactory())
                {
                    repository.DisableChangesTracking();

                    var orderEntities = await repository.GetCustomerOrdersByIdsAsync(orderIds, responseGroup);
                    foreach (var orderEntity in orderEntities)
                    {
                        var customerOrder = AbstractTypeFactory<CustomerOrder>.TryCreateInstance();
                        if (customerOrder != null)
                        {
                            customerOrder = orderEntity.ToModel(customerOrder) as CustomerOrder;

                            //Calculate totals only for full responseGroup
                            if (orderResponseGroup == CustomerOrderResponseGroup.Full)
                            {
                                _totalsCalculator.CalculateTotals(customerOrder);
                            }
                            await LoadOrderDependenciesAsync(customerOrder);

                            customerOrder.ReduceDetails(responseGroup);

                            retVal.Add(customerOrder);
                            cacheEntry.AddExpirationToken(OrderCacheRegion.CreateChangeToken(customerOrder));
                        }
                    }
                }
                return retVal.ToArray();
            });
        }

        public virtual async Task<CustomerOrder> GetByIdAsync(string orderId, string responseGroup = null)
        {
            var orders = await GetByIdsAsync(new[] { orderId }, responseGroup);
            return orders.FirstOrDefault();
        }

        public virtual async Task SaveChangesAsync(CustomerOrder[] orders)
        {
            var pkMap = new PrimaryKeyResolvingMap();
            var changedEntries = new List<GenericChangedEntry<CustomerOrder>>();

            using (var repository = _repositoryFactory())
            {
                var orderIds = orders.Where(x => !x.IsTransient()).Select(x => x.Id).ToArray();
                var dataExistOrders = await repository.GetCustomerOrdersByIdsAsync(orderIds, CustomerOrderResponseGroup.Full.ToString());
                foreach (var order in orders)
                {
                    await EnsureThatAllOperationsHaveNumber(order);
                    await LoadOrderDependenciesAsync(order);

                    var originalEntity = dataExistOrders.FirstOrDefault(x => x.Id == order.Id);
                    //Calculate order totals
                    _totalsCalculator.CalculateTotals(order);

                    var modifiedEntity = AbstractTypeFactory<CustomerOrderEntity>.TryCreateInstance()
                                                                                 .FromModel(order, pkMap) as CustomerOrderEntity;
                    if (originalEntity != null)
                    {
                        changedEntries.Add(new GenericChangedEntry<CustomerOrder>(order, (CustomerOrder)originalEntity.ToModel(AbstractTypeFactory<CustomerOrder>.TryCreateInstance()), EntryState.Modified));
                        modifiedEntity?.Patch(originalEntity);
                    }
                    else
                    {
                        repository.Add(modifiedEntity);
                        changedEntries.Add(new GenericChangedEntry<CustomerOrder>(order, EntryState.Added));
                    }
                }
                //Raise domain events
                await _eventPublisher.Publish(new OrderChangeEvent(changedEntries));
                await repository.UnitOfWork.CommitAsync();
                pkMap.ResolvePrimaryKeys();
            }

            await _eventPublisher.Publish(new OrderChangedEvent(changedEntries));
            ClearCache(orders);
        }

        public virtual async Task DeleteAsync(string[] ids)
        {
            var orders = await GetByIdsAsync(ids, CustomerOrderResponseGroup.Full.ToString());
            using (var repository = _repositoryFactory())
            {
                //Raise domain events before deletion
                var changedEntries = orders.Select(x => new GenericChangedEntry<CustomerOrder>(x, EntryState.Deleted));
                await _eventPublisher.Publish(new OrderChangeEvent(changedEntries));

                await repository.RemoveOrdersByIdsAsync(ids);

                await repository.UnitOfWork.CommitAsync();
                //Raise domain events after deletion
                await _eventPublisher.Publish(new OrderChangedEvent(changedEntries));
            }
            ClearCache(orders);
        }

        #endregion

        protected virtual async Task LoadOrderDependenciesAsync(CustomerOrder order)
        {
            if (order == null)
            {
                throw new ArgumentNullException(nameof(order));
            }

            var shippingMethods = await _shippingMethodsSearchService.SearchShippingMethodsAsync(new ShippingMethodsSearchCriteria { StoreId = order.StoreId });
            if (!shippingMethods.Results.IsNullOrEmpty())
            {
                foreach (var shipment in order.Shipments)
                {
                    shipment.ShippingMethod = shippingMethods.Results.FirstOrDefault(x => x.Code.EqualsInvariant(shipment.ShipmentMethodCode));
                }
            }
            var paymentMethods = await _paymentMethodSearchService.SearchPaymentMethodsAsync(new PaymentMethodsSearchCriteria { StoreId = order.StoreId });
            if (!paymentMethods.Results.IsNullOrEmpty())
            {
                foreach (var payment in order.InPayments)
                {
                    payment.PaymentMethod = paymentMethods.Results.FirstOrDefault(x => x.Code.EqualsInvariant(payment.GatewayCode));
                }
            }
        }

        protected virtual async Task EnsureThatAllOperationsHaveNumber(CustomerOrder order)
        {
            var store = await _storeService.GetByIdAsync(order.StoreId, StoreResponseGroup.StoreInfo.ToString());

            foreach (var operation in order.GetFlatObjectsListWithInterface<IOperation>())
            {
                if (operation.Number == null)
                {
                    var objectTypeName = operation.OperationType;

                    // take uppercase chars to form operation type, or just take 2 first chars. (CustomerOrder => CO, PaymentIn => PI, Shipment => SH)
                    var opType = string.Concat(objectTypeName.Select(c => char.IsUpper(c) ? c.ToString() : ""));
                    if (opType.Length < 2)
                    {
                        opType = objectTypeName.Substring(0, 2).ToUpper();
                    }

                    var numberTemplate = opType + "{0:yyMMdd}-{1:D5}";
                    if (store != null)
                    {
                        numberTemplate = store.Settings.GetSettingValue("Order." + objectTypeName + "NewNumberTemplate", numberTemplate);
                    }

                    operation.Number = _uniqueNumberGenerator.GenerateNumber(numberTemplate);
                }
            }
        }

        private void ClearCache(IEnumerable<CustomerOrder> orders)
        {
            OrderSearchCacheRegion.ExpireRegion();

            foreach (var order in orders)
            {
                OrderCacheRegion.ExpireOrder(order);
            }
        }
    }
}
