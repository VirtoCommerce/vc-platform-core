using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using VirtoCommerce.OrderModule.Core.Events;
using VirtoCommerce.OrderModule.Core.Model;
using VirtoCommerce.OrderModule.Core.Model.Search;
using VirtoCommerce.OrderModule.Core.Services;
using VirtoCommerce.OrderModule.Data.Model;
using VirtoCommerce.OrderModule.Data.Repositories;
using VirtoCommerce.Platform.Core.ChangeLog;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.DynamicProperties;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.Platform.Data.Infrastructure;
using VirtoCommerce.StoreModule.Core.Services;

namespace VirtoCommerce.OrderModule.Data.Services
{
    public class CustomerOrderServiceImpl : ICustomerOrderService
    {
        private readonly Func<IOrderRepository> _repositoryFactory;
        private readonly IEventPublisher _eventPublisher;
        private readonly IDynamicPropertyService _dynamicPropertyService;
        private readonly IStoreService _storeService;

        //protected IUniqueNumberGenerator UniqueNumberGenerator { get; }
        //protected IPaymentMethodsService PaymentMethodsService { get; }
        //protected IShippingMethodsService ShippingMethodsService { get; }
        private readonly IChangeLogService _changeLogService;
        private readonly ICustomerOrderTotalsCalculator _totalsCalculator;

        public CustomerOrderServiceImpl(Func<IOrderRepository> orderRepositoryFactory/*, IUniqueNumberGenerator uniqueNumberGenerator*/, IDynamicPropertyService dynamicPropertyService,
            //, IShippingMethodsService shippingMethodsService, IPaymentMethodsService paymentMethodsService,
                                       IStoreService storeService, IChangeLogService changeLogService, IEventPublisher eventPublisher, ICustomerOrderTotalsCalculator totalsCalculator)
        {
            _repositoryFactory = orderRepositoryFactory;
            //UniqueNumberGenerator = uniqueNumberGenerator;
            _eventPublisher = eventPublisher;
            _dynamicPropertyService = dynamicPropertyService;
            //ShippingMethodsService = shippingMethodsService;
            //PaymentMethodsService = paymentMethodsService;
            _storeService = storeService;
            _changeLogService = changeLogService;
            _totalsCalculator = totalsCalculator;
        }

        
        

        #region ICustomerOrderService Members

        public virtual async Task SaveChangesAsync(CustomerOrder[] orders)
        {
            var pkMap = new PrimaryKeyResolvingMap();
            var changedEntries = new List<GenericChangedEntry<CustomerOrder>>();

            using (var repository = _repositoryFactory())
            {
                var dataExistOrders = await repository.GetCustomerOrdersByIdsAsync(orders.Where(x => !x.IsTransient()).Select(x => x.Id).ToArray(), CustomerOrderResponseGroup.Full);
                foreach (var order in orders)
                {
                    await EnsureThatAllOperationsHaveNumber(order);
                    LoadOrderDependencies(order);

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

            //Save dynamic properties
            foreach (var order in orders)
            {
                await _dynamicPropertyService.SaveDynamicPropertyValuesAsync(order);
            }
            //Raise domain events
            await _eventPublisher.Publish(new OrderChangedEvent(changedEntries));
        }

        public virtual async Task<CustomerOrder[]> GetByIdsAsync(string[] orderIds, string responseGroup = null)
        {
            var retVal = new List<CustomerOrder>();
            var orderResponseGroup = EnumUtility.SafeParse(responseGroup, CustomerOrderResponseGroup.Full);

            using (var repository = _repositoryFactory())
            {
                repository.DisableChangesTracking();

                var orderEntities = await repository.GetCustomerOrdersByIdsAsync(orderIds, orderResponseGroup);
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
                        LoadOrderDependencies(customerOrder);
                        retVal.Add(customerOrder);
                    }
                }
            }

            await _dynamicPropertyService.LoadDynamicPropertyValuesAsync(retVal.ToArray<IHasDynamicProperties>());
            return retVal.ToArray();
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

                foreach (var order in orders)
                {
                    await _dynamicPropertyService.DeleteDynamicPropertyValuesAsync(order);
                }

                await repository.UnitOfWork.CommitAsync();
                //Raise domain events after deletion
                await _eventPublisher.Publish(new OrderChangedEvent(changedEntries));
            }
        }

        

        #endregion

        protected virtual void LoadOrderDependencies(CustomerOrder order)
        {
            //TODO
            //if (order == null)
            //{
            //    throw new ArgumentNullException(nameof(order));
            //}
            //var shippingMethods = ShippingMethodsService.GetAllShippingMethods();
            //if (!shippingMethods.IsNullOrEmpty())
            //{
            //    foreach (var shipment in order.Shipments)
            //    {
            //        shipment.ShippingMethod = shippingMethods.FirstOrDefault(x => x.Code.EqualsInvariant(shipment.ShipmentMethodCode));
            //    }
            //}
            //var paymentMethods = PaymentMethodsService.GetAllPaymentMethods();
            //if (!paymentMethods.IsNullOrEmpty())
            //{
            //    foreach (var payment in order.InPayments)
            //    {
            //        payment.PaymentMethod = paymentMethods.FirstOrDefault(x => x.Code.EqualsInvariant(payment.GatewayCode));
            //    }
            //}
        }

        protected virtual async Task EnsureThatAllOperationsHaveNumber(CustomerOrder order)
        {
            var store = await _storeService.GetByIdAsync(order.StoreId);

            foreach (var operation in order.GetFlatObjectsListWithInterface<Domain.Commerce.Model.IOperation>())
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

                    //TODO
                    //operation.Number = UniqueNumberGenerator.GenerateNumber(numberTemplate);
                }
            }
        }
    }
}
