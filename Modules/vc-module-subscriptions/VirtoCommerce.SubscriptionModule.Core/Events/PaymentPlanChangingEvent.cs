using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.SubscriptionModule.Core.Model;

namespace VirtoCommerce.SubscriptionModule.Core.Events
{
    public class PaymentPlanChangingEvent : GenericChangedEntryEvent<PaymentPlan>
    {
        public PaymentPlanChangingEvent(IEnumerable<GenericChangedEntry<PaymentPlan>> changedEntries)
           : base(changedEntries)
        {
        }
    }
}
