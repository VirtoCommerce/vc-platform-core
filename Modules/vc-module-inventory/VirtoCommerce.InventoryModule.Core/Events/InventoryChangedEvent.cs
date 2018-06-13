using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.Platform.Core.Domain;
using VirtoCommerce.InventoryModule.Core.Model;

namespace VirtoCommerce.Domain.Inventory.Events
{
    public class InventoryChangedEvent : GenericChangedEntryEvent<InventoryInfo>
    {
        public InventoryChangedEvent(IEnumerable<GenericChangedEntry<InventoryInfo>> changedEntries)
           : base(changedEntries)
        {
        }
    }
}
