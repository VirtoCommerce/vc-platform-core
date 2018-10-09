using System.Collections.Generic;
using VirtoCommerce.OrdersModule.Core.Model;
using VirtoCommerce.Platform.Core.Events;

namespace VirtoCommerce.OrdersModule.Core.Events
{
    public class OrderChangeEvent : GenericChangedEntryEvent<CustomerOrder>
    {
        public OrderChangeEvent(IEnumerable<GenericChangedEntry<CustomerOrder>> changedEntries)
           : base(changedEntries)
        {
        }
    }
}
