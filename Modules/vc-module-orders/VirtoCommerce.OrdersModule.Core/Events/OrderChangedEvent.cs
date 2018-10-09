using System.Collections.Generic;
using VirtoCommerce.OrdersModule.Core.Model;
using VirtoCommerce.Platform.Core.Events;

namespace VirtoCommerce.OrdersModule.Core.Events
{
    public class OrderChangedEvent : GenericChangedEntryEvent<CustomerOrder>
    {
        public OrderChangedEvent(IEnumerable<GenericChangedEntry<CustomerOrder>> changedEntries)
       : base(changedEntries)
        {
        }
       
    }
}
