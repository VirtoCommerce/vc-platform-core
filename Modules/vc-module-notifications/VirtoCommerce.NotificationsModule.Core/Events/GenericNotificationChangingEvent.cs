using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Domain;
using VirtoCommerce.Platform.Core.Events;

namespace VirtoCommerce.NotificationsModule.Core.Events
{
    /// <summary>
    /// Event generated before notifications entities were change
    /// </summary>
    public class GenericNotificationChangingEvent<T> : ChangeEntryEvent<T>
    {
        public GenericNotificationChangingEvent(IEnumerable<ChangedEntry<T>> changedEntries) : base(changedEntries)
        {
        }
    }
}
