using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.PricingModule.Core.Model;

namespace VirtoCommerce.PricingModule.Core.Events
{
    public class PriceChangedEvent : ChangesLogEvent<Price>
    {
        public PriceChangedEvent(IEnumerable<GenericChangedEntry<Price>> changedEntries)
            : base(changedEntries)
        {
        }
    }
}
