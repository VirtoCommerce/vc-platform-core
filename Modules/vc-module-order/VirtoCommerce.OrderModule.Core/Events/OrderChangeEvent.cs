using System.Collections.Generic;
using VirtoCommerce.OrderModule.Core.Model;
using VirtoCommerce.Platform.Core.Events;

namespace VirtoCommerce.OrderModule.Core.Events
{
    public class OrderChangeEvent : GenericChangedEntryEvent<CustomerOrder>
    {
        public OrderChangeEvent(IEnumerable<GenericChangedEntry<CustomerOrder>> changedEntries)
           : base(changedEntries)
        {
        }
    }
}
