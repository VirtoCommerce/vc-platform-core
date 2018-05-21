using System;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.NotificationsModule.Core.Model;
using VirtoCommerce.NotificationsModule.Core.Services;

namespace VirtoCommerce.NotificationsModule.Data.Senders
{
    public class NotificationMessageSenderProviderFactory : INotificationMessageSenderProviderFactory
    {
        private readonly Dictionary<string, INotificationMessageSender> _dictionary;
        private readonly IEnumerable<INotificationMessageSender> _senders;

        public NotificationMessageSenderProviderFactory(IEnumerable<INotificationMessageSender> senders)
        {
            _dictionary = new Dictionary<string, INotificationMessageSender>();
            _senders = senders;
        }

        public void RegisterSenderForType<T1, T2>() where T1 : Notification where T2 : INotificationMessageSender
        {
            var sender = _senders.First(s => s.GetType() == typeof(T2));
            _dictionary.Add(typeof(T1).Name, sender);
        }

        public INotificationMessageSender GetSenderForNotificationType(string type)
        {
            if (_dictionary.TryGetValue(type, out var senderService))
            {
                return senderService;
            }

            throw new NotSupportedException($"Unsupported notification type \"{type}\".");
        }
    }
}
