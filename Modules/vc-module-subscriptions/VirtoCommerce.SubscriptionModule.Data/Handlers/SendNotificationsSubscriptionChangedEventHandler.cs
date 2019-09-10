using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using VirtoCommerce.CustomerModule.Core.Services;
using VirtoCommerce.NotificationsModule.Core.Extensions;
using VirtoCommerce.NotificationsModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.StoreModule.Core.Model;
using VirtoCommerce.StoreModule.Core.Services;
using VirtoCommerce.SubscriptionModule.Core.Events;
using VirtoCommerce.SubscriptionModule.Core.Model;
using VirtoCommerce.SubscriptionModule.Data.Notifications;

namespace VirtoCommerce.SubscriptionModule.Data.Handlers
{
    public class SendNotificationsSubscriptionChangedEventHandler : IEventHandler<SubscriptionChangedEvent>
    {
        private readonly INotificationSearchService _notificationSearchService;
        private readonly INotificationSender _notificationSender;
        private readonly IStoreService _storeService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMemberService _memberService;

        public SendNotificationsSubscriptionChangedEventHandler(INotificationSearchService notificationSearchService, INotificationSender notificationSender,
            IStoreService storeService, UserManager<ApplicationUser> userManager, IMemberService memberService)
        {
            _notificationSearchService = notificationSearchService;
            _notificationSender = notificationSender;
            _storeService = storeService;
            _userManager = userManager;
            _memberService = memberService;
        }

        public virtual async Task Handle(SubscriptionChangedEvent message)
        {
            foreach (var changedEntry in message.ChangedEntries)
            {
                await TryToSendNotificationsAsync(changedEntry);
            }
        }

        protected virtual async Task TryToSendNotificationsAsync(GenericChangedEntry<Subscription> changedEntry)
        {
            //Collection of order notifications
            var notifications = new List<SubscriptionEmailNotificationBase>();

            if (IsSubscriptionCanceled(changedEntry))
            {
                //Resolve SubscriptionCanceledEmailNotification with template defined on store level
                var notification = await _notificationSearchService.GetNotificationAsync<SubscriptionCanceledEmailNotification>(new TenantIdentity(changedEntry.NewEntry.StoreId, nameof(Store)));
                if (notification != null)
                {
                    notifications.Add(notification);
                }
            }

            if (changedEntry.EntryState == EntryState.Added)
            {
                //Resolve NewSubscriptionEmailNotification with template defined on store level
                var notification = await _notificationSearchService.GetNotificationAsync<NewSubscriptionEmailNotification>(new TenantIdentity(changedEntry.NewEntry.StoreId, nameof(Store)));
                if (notification != null)
                {
                    notifications.Add(notification);
                }
            }

            foreach (var notification in notifications)
            {
                await SetNotificationParametersAsync(notification, changedEntry.NewEntry);
                await _notificationSender.SendNotificationAsync(notification);
            }
        }

        /// <summary>
        /// Set base notification parameters (sender, recipient, isActive)
        /// </summary>
        protected virtual async Task SetNotificationParametersAsync(SubscriptionEmailNotificationBase notification, Subscription subscription)
        {
            var store = await _storeService.GetByIdAsync(subscription.StoreId, StoreResponseGroup.StoreInfo.ToString());

            notification.IsActive = true;
            notification.Subscription = subscription;

            notification.From = store.Email;
            notification.To = await GetSubscriptionRecipientEmailAsync(subscription);
            notification.LanguageCode = subscription.CustomerOrderPrototype.LanguageCode;

            //Link notification to subscription to getting notification history for each subscription individually
            notification.TenantIdentity = new TenantIdentity(subscription.Id, typeof(Subscription).Name);
        }

        protected virtual bool IsSubscriptionCanceled(GenericChangedEntry<Subscription> changedEntry)
        {
            var result = changedEntry.OldEntry != null &&
                      changedEntry.OldEntry.IsCancelled != changedEntry.NewEntry.IsCancelled &&
                      changedEntry.NewEntry.IsCancelled;

            return result;
        }

        protected virtual async Task<string> GetSubscriptionRecipientEmailAsync(Subscription subscription)
        {
            return await GetSubscriptionOrderEmailAsync(subscription) ?? await GetCustomerEmailAsync(subscription.CustomerId);
        }

        protected virtual Task<string> GetSubscriptionOrderEmailAsync(Subscription subscription)
        {
            var email = subscription.CustomerOrderPrototype.Addresses?.Select(x => x.Email).FirstOrDefault();
            return Task.FromResult(email);
        }

        protected virtual async Task<string> GetCustomerEmailAsync(string customerId)
        {
            // try to find user
            var user = await _userManager.FindByIdAsync(customerId);

            // Try to find contact
            var contact = await _memberService.GetByIdAsync(customerId);
            if (contact == null && user != null)
            {
                contact = await _memberService.GetByIdAsync(user.MemberId);
            }

            return contact?.Emails?.FirstOrDefault() ?? user?.Email;
        }
    }
}
