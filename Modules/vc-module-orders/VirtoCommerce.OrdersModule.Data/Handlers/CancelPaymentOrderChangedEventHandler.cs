using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.CoreModule.Core.Payment;
using VirtoCommerce.OrdersModule.Core.Events;
using VirtoCommerce.OrdersModule.Core.Model;
using VirtoCommerce.OrdersModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.StoreModule.Core.Services;

namespace VirtoCommerce.OrdersModule.Data.Handlers
{
    public class CancelPaymentOrderChangedEventHandler : IEventHandler<OrderChangedEvent>
    {
        private readonly IStoreService _storeService;
        private readonly ICustomerOrderService _orderService;

        public CancelPaymentOrderChangedEventHandler(IStoreService storeService, ICustomerOrderService customerOrderService)
        {
            _storeService = storeService;
            _orderService = customerOrderService;
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
            var store = await _storeService.GetByIdAsync(changedEntry.NewEntry.StoreId);
            //Try to load payment methods for payments
            foreach (var payment in changedEntry.NewEntry.InPayments)
            {
                payment.PaymentMethod = store.PaymentMethods.FirstOrDefault(p => p.Code.EqualsInvariant(payment.GatewayCode));
            }

            var toCancelPayments = new List<PaymentIn>();
            var isOrderCancelled = !changedEntry.OldEntry.IsCancelled && changedEntry.NewEntry.IsCancelled;
            if (isOrderCancelled)
            {
                toCancelPayments = changedEntry.NewEntry.InPayments?.ToList();
            }
            else
            {
                foreach (var canceledPayment in changedEntry.NewEntry?.InPayments.Where(x => x.IsCancelled) ?? Enumerable.Empty<PaymentIn>())
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
                    payment.PaymentMethod?.VoidProcessPayment(new VoidProcessPaymentEvaluationContext { PaymentId = payment.Id, OrderId = order.Id });
                }
                else if (payment.PaymentStatus == PaymentStatus.Paid)
                {
                    payment.PaymentMethod?.RefundProcessPayment(new RefundProcessPaymentEvaluationContext { PaymentId = payment.Id, OrderId = order.Id });
                }
                else
                {
                    payment.PaymentStatus = PaymentStatus.Cancelled;
                    payment.IsCancelled = true;
                    payment.CancelledDate = DateTime.UtcNow;
                }
            }
        }
    }
}
