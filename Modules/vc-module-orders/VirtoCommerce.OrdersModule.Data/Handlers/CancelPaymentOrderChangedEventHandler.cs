using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.OrdersModule.Core.Events;
using VirtoCommerce.OrdersModule.Core.Model;
using VirtoCommerce.OrdersModule.Core.Services;
using VirtoCommerce.PaymentModule.Core.Model;
using VirtoCommerce.PaymentModule.Core.Model.Search;
using VirtoCommerce.PaymentModule.Core.Services;
using VirtoCommerce.PaymentModule.Model.Requests;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.StoreModule.Core.Model;
using VirtoCommerce.StoreModule.Core.Services;

namespace VirtoCommerce.OrdersModule.Data.Handlers
{
    public class CancelPaymentOrderChangedEventHandler : IEventHandler<OrderChangedEvent>
    {
        private readonly IStoreService _storeService;
        private readonly ICustomerOrderService _orderService;
        private readonly IPaymentMethodsSearchService _paymentMethodsSearchService;

        public CancelPaymentOrderChangedEventHandler(
            IStoreService storeService,
            ICustomerOrderService customerOrderService,
            IPaymentMethodsSearchService paymentMethodsSearchService
            )
        {
            _storeService = storeService;
            _orderService = customerOrderService;
            _paymentMethodsSearchService = paymentMethodsSearchService;
        }


        public virtual async Task Handle(OrderChangedEvent message)
        {
            foreach (var changedEntry in message.ChangedEntries.Where(x => x.EntryState == EntryState.Modified))
            {
                await TryToCancelOrder(changedEntry);
            }
        }

        protected virtual async Task TryToCancelOrder(GenericChangedEntry<CustomerOrder> changedEntry)
        {
            var store = await _storeService.GetByIdAsync(changedEntry.NewEntry.StoreId, StoreResponseGroup.StoreInfo.ToString());

            //Try to load payment methods for payments
            var gatewayCodes = changedEntry.NewEntry.InPayments.Select(x => x.GatewayCode).ToArray();
            var paymentMethods = await GetPaymentMethodsAsync(store.Id, gatewayCodes);
            foreach (var payment in changedEntry.NewEntry.InPayments)
            {
                payment.PaymentMethod = paymentMethods.FirstOrDefault(x => x.Code == payment.GatewayCode);
            }

            var toCancelPayments = new List<PaymentIn>();
            var isOrderCancelled = !changedEntry.OldEntry.IsCancelled && changedEntry.NewEntry.IsCancelled;
            if (isOrderCancelled)
            {
                toCancelPayments = changedEntry.NewEntry.InPayments?.ToList();
            }
            else
            {
                foreach (var canceledPayment in changedEntry.NewEntry?.InPayments.Where(x => x.IsCancelled))
                {
                    var oldSamePayment = changedEntry.OldEntry?.InPayments.FirstOrDefault(x => x == canceledPayment);
                    if (oldSamePayment != null && !oldSamePayment.IsCancelled)
                    {
                        toCancelPayments.Add(canceledPayment);
                    }
                }
            }
            TryToCancelOrderPayments(toCancelPayments, changedEntry.NewEntry);
            if (!toCancelPayments.IsNullOrEmpty())
            {
                await _orderService.SaveChangesAsync(new[] { changedEntry.NewEntry });
            }
        }

        protected virtual void TryToCancelOrderPayments(IEnumerable<PaymentIn> toCancelPayments, CustomerOrder order)
        {
            foreach (var payment in toCancelPayments ?? Enumerable.Empty<PaymentIn>())
            {
                if (payment.PaymentStatus == PaymentStatus.Authorized)
                {
                    payment.PaymentMethod?.VoidProcessPayment(new VoidPaymentRequest { PaymentId = payment.Id, OrderId = order.Id });
                }
                else if (payment.PaymentStatus == PaymentStatus.Paid)
                {
                    payment.PaymentMethod?.RefundProcessPayment(new RefundPaymentRequest { PaymentId = payment.Id, OrderId = order.Id });
                }
                else
                {
                    payment.PaymentStatus = PaymentStatus.Cancelled;
                    payment.IsCancelled = true;
                    payment.CancelledDate = DateTime.UtcNow;
                }
            }
        }

        protected virtual async Task<ICollection<PaymentMethod>> GetPaymentMethodsAsync(string storeId, string[] codes)
        {
            var criteria = new PaymentMethodsSearchCriteria
            {
                IsActive = true,
                StoreId = storeId,
                Codes = codes,
                Take = int.MaxValue
            };

            var searchResult = await _paymentMethodsSearchService.SearchPaymentMethodsAsync(criteria);
            var paymentMethod = searchResult.Results;

            return paymentMethod;
        }
    }
}
