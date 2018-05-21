using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Domain;
using VirtoCommerce.Platform.Core.Events;

namespace VirtoCommerce.NotificationsModule.Core.Events
{
    /// <summary>
    /// Event generated after notifications entities were change
    /// </summary>
    public class GenericNotificationChangedEvent<T> : ChangeEntryEvent<T>
    {
        public GenericNotificationChangedEvent(IEnumerable<ChangedEntry<T>> changedEntries) : base(changedEntries)
        {
        }
    }
}
