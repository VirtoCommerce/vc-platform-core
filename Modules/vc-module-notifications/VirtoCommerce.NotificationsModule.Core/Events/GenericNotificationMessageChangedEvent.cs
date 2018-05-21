using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Domain;
using VirtoCommerce.Platform.Core.Events;

namespace VirtoCommerce.NotificationsModule.Core.Events
{
    public class GenericNotificationMessageChangedEvent<T> : ChangeEntryEvent<T>
    {
        public GenericNotificationMessageChangedEvent(IEnumerable<ChangedEntry<T>> changedEntries) : base(changedEntries)
        {
        }
    }
}
