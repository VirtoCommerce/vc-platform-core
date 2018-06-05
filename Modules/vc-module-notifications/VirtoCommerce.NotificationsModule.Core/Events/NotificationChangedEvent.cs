using System.Collections.Generic;
using VirtoCommerce.NotificationsModule.Core.Model;
using VirtoCommerce.Platform.Core.Domain;
using VirtoCommerce.Platform.Core.Events;

namespace VirtoCommerce.NotificationsModule.Core.Events
{
    public class NotificationChangedEvent : GenericChangedEntryEvent<Notification>
    {
        public NotificationChangedEvent(IEnumerable<GenericChangedEntry<Notification>> changedEntries)
            : base(changedEntries)
        {
        }
    }
}
