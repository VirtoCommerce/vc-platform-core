using System;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using VirtoCommerce.OrdersModule.Core.Events;
using VirtoCommerce.OrdersModule.Core.Model;
using VirtoCommerce.OrdersModule.Core.Services;
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

        public virtual Task Handle(OrderChangedEvent message)
        {
            var addedOrders = message.ChangedEntries.Where(x => x.EntryState == EntryState.Added).Select(e => e.NewEntry).ToArray();
            BackgroundJob.Enqueue(() => HandleOrderChangesInBackground(addedOrders));

            return Task.CompletedTask;
        }

        [DisableConcurrentExecution(60 * 60 * 24)]
        public virtual void HandleOrderChangesInBackground(CustomerOrder[] orders)
        {
            foreach (var order in orders)
            {
                HandleOrderChanges(order);
            }
        }

        public void HandleOrderChanges(CustomerOrder order)
        {
            HandleOrderChangesAsync(order).GetAwaiter().GetResult();
        }

        protected virtual async Task HandleOrderChangesAsync(CustomerOrder customerOrder)
        {
            //Prevent creating subscription for customer orders with other operation type (it is need for preventing to handling  subscription prototype and recurring order creations)
            if (!customerOrder.IsPrototype && string.IsNullOrEmpty(customerOrder.SubscriptionId))
            {
                try
                {
                    var subscription = await _subscriptionBuilder.TryCreateSubscriptionFromOrderAsync(customerOrder);
                    if (subscription != null)
                    {
                        await _subscriptionBuilder.TakeSubscription(subscription).ActualizeAsync();
                        await _subscriptionService.SaveSubscriptionsAsync(new[] { subscription });
                        //Link subscription with customer order
                        customerOrder.SubscriptionId = subscription.Id;
                        customerOrder.SubscriptionNumber = subscription.Number;
                        //Save order changes
                        await _customerOrderService.SaveChangesAsync(new[] { customerOrder });
                    }
                }
                catch (Exception ex)
                {
                    throw new CreateSubscriptionException(ex);
                }
            }
        }
    }
}
