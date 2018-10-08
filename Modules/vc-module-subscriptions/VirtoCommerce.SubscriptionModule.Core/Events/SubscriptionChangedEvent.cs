using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.SubscriptionModule.Core.Model;

namespace VirtoCommerce.SubscriptionModule.Core.Events
{
    public class SubscriptionChangedEvent : GenericChangedEntryEvent<Subscription>
    {
        public SubscriptionChangedEvent(IEnumerable<GenericChangedEntry<Subscription>> changedEntries)
           : base(changedEntries)
        {
        }
    }
}
