using System.Collections.Generic;
using VirtoCommerce.NotificationsModule.Core.Model;
using VirtoCommerce.Platform.Core.Domain;
using VirtoCommerce.Platform.Core.Events;

namespace VirtoCommerce.NotificationsModule.Core.Events
{
    public class NotificationMessageChangingEvent : GenericChangedEntryEvent<NotificationMessage>
    {
        public NotificationMessageChangingEvent(IEnumerable<GenericChangedEntry<NotificationMessage>> changedEntries)
            : base(changedEntries)
        {
        }
    }
}
