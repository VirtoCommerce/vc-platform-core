using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.StoreModule.Core.Services;
using VirtoCommerce.SubscriptionModule.Core.Events;
using VirtoCommerce.SubscriptionModule.Core.Model;
using VirtoCommerce.SubscriptionModule.Data.Notifications;

namespace VirtoCommerce.SubscriptionModule.Data.Handlers
{
    public class SendNotificationsSubscriptionChangedEventHandler : IEventHandler<SubscriptionChangedEvent>
    {
        private readonly INotificationManager _notificationManager;
        private readonly IStoreService _storeService;
        private readonly ISecurityService _securityService;
        private readonly IMemberService _memberService;

        public SendNotificationsSubscriptionChangedEventHandler(INotificationManager notificationManager, IStoreService storeService, ISecurityService securityService, IMemberService memberService)
        {
            _notificationManager = notificationManager;
            _storeService = storeService;
            _securityService = securityService;
            _memberService = memberService;
        }

        public async virtual Task Handle(SubscriptionChangedEvent message)
        {
            foreach (var changedEntry in message.ChangedEntries)
            {
                await TryToSendNotificationsAsync(changedEntry);
            }
        }

        protected async virtual Task TryToSendNotificationsAsync(GenericChangedEntry<Subscription> changedEntry)
        {
            //Collection of order notifications
            var notifications = new List<SubscriptionEmailNotificationBase>();

            if (IsSubscriptionCanceled(changedEntry))
            {
                //Resolve SubscriptionCanceledEmailNotification with template defined on store level
                var notification = _notificationManager.GetNewNotification<SubscriptionCanceledEmailNotification>(changedEntry.NewEntry.StoreId, "Store", changedEntry.NewEntry.CustomerOrderPrototype.LanguageCode);
                notifications.Add(notification);
            }

            if (changedEntry.EntryState == EntryState.Added)
            {
                //Resolve NewSubscriptionEmailNotification with template defined on store level
                var notification = _notificationManager.GetNewNotification<NewSubscriptionEmailNotification>(changedEntry.NewEntry.StoreId, "Store", changedEntry.NewEntry.CustomerOrderPrototype.LanguageCode);
                notifications.Add(notification);
            }

            foreach (var notification in notifications)
            {
                await SetNotificationParametersAsync(notification, changedEntry.NewEntry);
                _notificationManager.ScheduleSendNotification(notification);
            }
        }

        /// <summary>
        /// Set base notification parameters (sender, recipient, isActive)
        /// </summary>
        protected virtual async Task SetNotificationParametersAsync(SubscriptionEmailNotificationBase notification, Subscription subscription)
        {
            var store = _storeService.GetById(subscription.StoreId);

            notification.Subscription = subscription;
            notification.Sender = store.Email;
            notification.IsActive = true;
            //Link notification to subscription to getting notification history for each subscription individually
            notification.ObjectId = subscription.Id;
            notification.ObjectTypeId = typeof(Subscription).Name;
            notification.Recipient = await GetSubscriptionRecipientEmailAsync(subscription);           
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
            //get recipient email from order address as default
            var email = subscription.CustomerOrderPrototype.Addresses?.Select(x => x.Email).FirstOrDefault();
            //try to find user
            var user = await _securityService.FindByIdAsync(subscription.CustomerId, UserDetails.Reduced);
            //Try to find contact 
            var contact = _memberService.GetByIds(new[] { subscription.CustomerId }).OfType<Contact>().FirstOrDefault();
            if (contact == null && user != null)
            {
                contact = _memberService.GetByIds(new[] { user.MemberId }).OfType<Contact>().FirstOrDefault();
            }
            email = contact?.Emails?.FirstOrDefault() ?? email ?? user?.Email;
            return email;
        }
    }


}
