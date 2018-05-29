using System.Collections.Generic;
using VirtoCommerce.NotificationsModule.Core.Model;
using VirtoCommerce.Platform.Core.Domain;
using VirtoCommerce.Platform.Core.Events;

namespace VirtoCommerce.NotificationsModule.Core.Events
{
    public class NotificationMessageChangedEvent : GenericChangedEntryEvent<NotificationMessage>
    {
        public NotificationMessageChangedEvent(IEnumerable<GenericChangedEntry<NotificationMessage>> changedEntries)
            : base(changedEntries)
        {
        }
    }
}
