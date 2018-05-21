using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Domain;
using VirtoCommerce.Platform.Core.Events;

namespace VirtoCommerce.NotificationsModule.Core.Events
{
    public class GenericNotificationMessageChangingEvent<T> : ChangeEntryEvent<T>
    {
        public GenericNotificationMessageChangingEvent(IEnumerable<ChangedEntry<T>> changedEntries) : base(changedEntries)
        {
        }
    }
}
