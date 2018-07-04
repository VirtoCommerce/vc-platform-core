using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.StoreModule.Core.Model;

namespace VirtoCommerce.StoreModule.Core.Events
{
    public class StoreChangedEvent : GenericChangedEntryEvent<Store>
    {
        public StoreChangedEvent(IEnumerable<GenericChangedEntry<Store>> changedEntries) : base(changedEntries)
        {
        }
    }
}
