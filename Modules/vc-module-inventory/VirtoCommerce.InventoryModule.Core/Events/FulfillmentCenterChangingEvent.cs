using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.Platform.Core.Domain;
using VirtoCommerce.InventoryModule.Core.Model;

namespace VirtoCommerce.Domain.Inventory.Events
{
    public class FulfillmentCenterChangingEvent : GenericChangedEntryEvent<FulfillmentCenter>
    {       
        public FulfillmentCenterChangingEvent(IEnumerable<GenericChangedEntry<FulfillmentCenter>> changedEntries)
           : base(changedEntries)
        {
        }
    }
}
