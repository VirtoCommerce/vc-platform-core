using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using VirtoCommerce.CoreModule.Core.Payment;
using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.CustomerModule.Core.Services;
using VirtoCommerce.NotificationsModule.Core.Model;
using VirtoCommerce.NotificationsModule.Core.Services;
using VirtoCommerce.OrdersModule.Core;
using VirtoCommerce.OrdersModule.Core.Events;
using VirtoCommerce.OrdersModule.Core.Model;
using VirtoCommerce.OrdersModule.Core.Notifications;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.StoreModule.Core.Services;

namespace VirtoCommerce.OrdersModule.Data.Handlers
{
    public class SendNotificationsOrderChangedEventHandler : IEventHandler<OrderChangedEvent>
    {
        private readonly INotificationService _notificationService;
        private readonly INotificationSender _notificationSender;
        private readonly IStoreService _storeService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMemberService _memberService;
        private readonly ISettingsManager _settingsManager;

        public SendNotificationsOrderChangedEventHandler(INotificationSender notificationSender, IStoreService storeService
            , UserManager<ApplicationUser> userManager
            , IMemberService memberService
            , ISettingsManager settingsManager
            , INotificationService notificationService
            )
        {
            _notificationSender = notificationSender;
            _storeService = storeService;
            _userManager = userManager;
            _memberService = memberService;
            _settingsManager = settingsManager;
            _notificationService = notificationService;
        }

        public virtual async Task Handle(OrderChangedEvent message)
        {
            if (_settingsManager.GetValue(ModuleConstants.Settings.General.SendOrderNotifications.Name, true))
            {
                foreach (var changedEntry in message.ChangedEntries)
                {
                    await TryToSendOrderNotificationsAsync(changedEntry);
                }
            }
        }

        protected virtual async Task TryToSendOrderNotificationsAsync(GenericChangedEntry<CustomerOrder> changedEntry)
        {
            // Collection of order notifications
            var notifications = new List<OrderEmailNotificationBase>();

            if (IsOrderCanceled(changedEntry))
            {
                var originalNotification = await _notificationService.GetByTypeAsync(nameof(CancelOrderEmailNotification));
                if (originalNotification is CancelOrderEmailNotification notification)
                {
                    notification.CustomerOrder = changedEntry.NewEntry;
                    notifications.Add(notification);
                }
            }

            if (changedEntry.EntryState == EntryState.Added && !changedEntry.NewEntry.IsPrototype)
            {
                var originalNotification = await _notificationService.GetByTypeAsync(nameof(OrderCreateEmailNotification));
                if (originalNotification is OrderCreateEmailNotification notification)
                {
                    notification.CustomerOrder = changedEntry.NewEntry;
                    notifications.Add(notification);
                }
            }

            if (HasNewStatus(changedEntry))
            {
                var originalNotification = await _notificationService.GetByTypeAsync(nameof(NewOrderStatusEmailNotification));
                if (originalNotification is NewOrderStatusEmailNotification notification)
                {
                    notification.CustomerOrder = changedEntry.NewEntry;
                    notification.NewStatus = changedEntry.NewEntry.Status;
                    notification.OldStatus = changedEntry.OldEntry.Status;
                    notifications.Add(notification);
                }
            }

            if (IsOrderPaid(changedEntry))
            {
                var originalNotification = await _notificationService.GetByTypeAsync(nameof(OrderPaidEmailNotification));
                if (originalNotification is OrderCreateEmailNotification notification)
                {
                    notification.CustomerOrder = changedEntry.NewEntry;
                    notifications.Add(notification);
                }
            }

            if (IsOrderSent(changedEntry))
            {
                var originalNotification = await _notificationService.GetByTypeAsync(nameof(OrderSentEmailNotification));
                if (originalNotification is OrderCreateEmailNotification notification)
                {
                    notification.CustomerOrder = changedEntry.NewEntry;
                    notifications.Add(notification);
                }
            }

            foreach (var notification in notifications)
            {
                notification.CustomerOrder = changedEntry.NewEntry;
                await SetNotificationParametersAsync(notification, changedEntry);
                await _notificationSender.SendNotificationAsync(notification, changedEntry.NewEntry.LanguageCode);
            }
        }

        /// <summary>
        /// Is order canceled
        /// </summary>
        /// <param name="changedEntry"></param>
        /// <returns></returns>
        protected virtual bool IsOrderCanceled(GenericChangedEntry<CustomerOrder> changedEntry)
        {
            var result = !changedEntry.OldEntry.IsCancelled && changedEntry.NewEntry.IsCancelled;
            return result;
        }

        /// <summary>
        /// The order has a new status
        /// </summary>
        /// <param name="changedEntry"></param>
        /// <returns></returns>
        protected virtual bool HasNewStatus(GenericChangedEntry<CustomerOrder> changedEntry)
        {
            var retVal = changedEntry.OldEntry.Status != changedEntry.NewEntry.Status;
            return retVal;
        }

        /// <summary>
        /// Is order fully paid
        /// </summary>
        /// <param name="changedEntry"></param>
        /// <returns></returns>
        protected virtual bool IsOrderPaid(GenericChangedEntry<CustomerOrder> changedEntry)
        {
            var oldPaidTotal = changedEntry.OldEntry.InPayments?.Where(x => x.PaymentStatus == PaymentStatus.Paid).Sum(x => x.Sum) ?? 0;
            var newPaidTotal = changedEntry.NewEntry.InPayments?.Where(x => x.PaymentStatus == PaymentStatus.Paid).Sum(x => x.Sum) ?? 0;
            return oldPaidTotal != newPaidTotal && changedEntry.NewEntry.Total <= newPaidTotal;
        }

        /// <summary>
        /// Is order fully send
        /// </summary>
        /// <param name="changedEntry"></param>
        /// <returns></returns>
        protected virtual bool IsOrderSent(GenericChangedEntry<CustomerOrder> changedEntry)
        {
            var oldSentShipmentsCount = changedEntry.OldEntry.Shipments?.Count(x => x.Status.EqualsInvariant("Send") || x.Status.EqualsInvariant("Sent")) ?? 0;
            var newSentShipmentsCount = changedEntry.NewEntry.Shipments?.Count(x => x.Status.EqualsInvariant("Send") || x.Status.EqualsInvariant("Sent")) ?? 0;
            return oldSentShipmentsCount == 0 && newSentShipmentsCount > 0;
        }

        /// <summary>
        /// Set base notification parameters (sender, recipient, isActive)
        /// </summary>
        /// <param name="notification"></param>
        /// <param name="changedEntry"></param>
        protected virtual async Task SetNotificationParametersAsync(Notification notification, GenericChangedEntry<CustomerOrder> changedEntry)
        {
            var order = changedEntry.NewEntry;
            var store = await _storeService.GetByIdAsync(order.StoreId);

            notification.IsActive = true;

            if (notification is EmailNotification emailNotification)
            {
                emailNotification.From = store.Email;
                emailNotification.To = await GetOrderRecipientEmailAsync(order);
            }

            // Allow to filter notification log either by customer order or by subscription
            if (string.IsNullOrEmpty(order.SubscriptionId))
            {
                notification.TenantIdentity = new TenantIdentity("CustomerOrder", order.Id);
            }
            else
            {
                notification.TenantIdentity = new TenantIdentity("Subscription", order.SubscriptionId);
            }
        }

        protected virtual async Task<string> GetOrderRecipientEmailAsync(CustomerOrder order)
        {
            var email = GetOrderAddressEmail(order) ?? await GetCustomerEmailAsync(order.CustomerId);
            return email;
        }

        protected virtual string GetOrderAddressEmail(CustomerOrder order)
        {
            var email = order.Addresses?.Select(x => x.Email).FirstOrDefault(x => !string.IsNullOrEmpty(x));
            return email;
        }

        protected virtual async Task<string> GetCustomerEmailAsync(string customerId)
        {
            var user = await _userManager.FindByIdAsync(customerId);

            var contact = user != null
                ? (await _memberService.GetByIdsAsync(new[] { user.MemberId })).OfType<Contact>().FirstOrDefault()
                : (await _memberService.GetByIdsAsync(new[] { customerId })).OfType<Contact>().FirstOrDefault();

            var email = contact?.Emails?.FirstOrDefault(x => !string.IsNullOrEmpty(x)) ?? user?.Email;
            return email;
        }
    }
}
