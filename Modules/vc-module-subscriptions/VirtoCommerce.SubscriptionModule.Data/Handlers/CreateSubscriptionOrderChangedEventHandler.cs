using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.SubscriptionModule.Core.Services;
using VirtoCommerce.SubscriptionModule.Data.Exceptions;

namespace VirtoCommerce.SubscriptionModule.Data.Handlers
{
    public class CreateSubscriptionOrderChangedEventHandler : IEventHandler<OrderChangedEvent>
    {
        private readonly ISubscriptionBuilder _subscriptionBuilder;
        private readonly ISubscriptionService _subscriptionService;
        private readonly ICustomerOrderService _customerOrderService;
        public CreateSubscriptionOrderChangedEventHandler(ISubscriptionBuilder subscriptionBuilder, ISubscriptionService subscriptionService, ICustomerOrderService customerOrderService)
        {
            _subscriptionBuilder = subscriptionBuilder;
            _subscriptionService = subscriptionService;
            _customerOrderService = customerOrderService;
        }

        public async virtual Task Handle(OrderChangedEvent message)
        {
            foreach (var changedEntry in message.ChangedEntries.Where(x => x.EntryState == EntryState.Added))
            {
                await HandleOrderChangesAsync(changedEntry);
            }
        }

        protected virtual Task HandleOrderChangesAsync(GenericChangedEntry<CustomerOrder> changedEntry)
        {
            var customerOrder = changedEntry.NewEntry;
            //Prevent creating subscription for customer orders with other operation type (it is need for preventing to handling  subscription prototype and recurring order creations)
            if (!customerOrder.IsPrototype && string.IsNullOrEmpty(customerOrder.SubscriptionId))
            {
                try
                {
                    var subscription = _subscriptionBuilder.TryCreateSubscriptionFromOrder(customerOrder);
                    if (subscription != null)
                    {
                        _subscriptionBuilder.TakeSubscription(subscription).Actualize();
                        _subscriptionService.SaveSubscriptions(new[] { subscription });
                        //Link subscription with customer order
                        customerOrder.SubscriptionId = subscription.Id;
                        customerOrder.SubscriptionNumber = subscription.Number;
                        //Save order changes
                        _customerOrderService.SaveChanges(new[] { customerOrder });
                    }
                }
                catch (Exception ex)
                {
                    throw new CreateSubscriptionException(ex);
                }
            }

            return Task.CompletedTask;
        }
    }
}
