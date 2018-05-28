using System.Collections.Generic;
using VirtoCommerce.NotificationsModule.Core.Model;
using VirtoCommerce.Platform.Core.Domain;
using VirtoCommerce.Platform.Core.Events;

namespace VirtoCommerce.NotificationsModule.Core.Events
{
    public class NotificationChangingEvent : GenericChangedEntryEvent<Notification>
    {
        public NotificationChangingEvent(IEnumerable<GenericChangedEntry<Notification>> changedEntries)
            : base(changedEntries)
        {
        }
    }
}
