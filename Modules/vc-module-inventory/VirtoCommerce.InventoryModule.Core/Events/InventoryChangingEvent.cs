using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.Platform.Core.Domain;
using VirtoCommerce.InventoryModule.Core.Model;

namespace VirtoCommerce.Domain.Inventory.Events
{
    public class InventoryChangingEvent : GenericChangedEntryEvent<InventoryInfo>
    {       
        public InventoryChangingEvent(IEnumerable<GenericChangedEntry<InventoryInfo>> changedEntries)
           : base(changedEntries)
        {
        }
    }
}
