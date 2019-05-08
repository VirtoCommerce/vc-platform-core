using System.Collections.Generic;
using VirtoCommerce.InventoryModule.Core.Model;
using VirtoCommerce.Platform.Core.Events;

namespace VirtoCommerce.InventoryModule.Core.Events
{
    public class InventoryChangedEvent : ChangesLogEvent<InventoryInfo>
    {
        public InventoryChangedEvent(IEnumerable<GenericChangedEntry<InventoryInfo>> changedEntries)
           : base(changedEntries)
        {
        }
    }
}
