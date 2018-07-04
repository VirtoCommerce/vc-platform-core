using System.Collections.Generic;
using VirtoCommerce.InventoryModule.Core.Model;
using VirtoCommerce.Platform.Core.Domain;
using VirtoCommerce.Platform.Core.Events;

namespace VirtoCommerce.InventoryModule.Core.Events
{
    public class FulfillmentCenterChangingEvent : GenericChangedEntryEvent<FulfillmentCenter>
    {       
        public FulfillmentCenterChangingEvent(IEnumerable<GenericChangedEntry<FulfillmentCenter>> changedEntries)
           : base(changedEntries)
        {
        }
    }
}
