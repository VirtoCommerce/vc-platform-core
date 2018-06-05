using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Domain;

namespace VirtoCommerce.Platform.Core.Events
{
    public class GenericChangedEntryEvent<T> : DomainEvent
    {
        public GenericChangedEntryEvent(IEnumerable<GenericChangedEntry<T>> changedEntries)
        {
            ChangedEntries = changedEntries;
        }

        public IEnumerable<GenericChangedEntry<T>> ChangedEntries { get; private set; }
    }
}
