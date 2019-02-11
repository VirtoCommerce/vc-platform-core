using System.Collections.Generic;
using VirtoCommerce.MarketingModule.Core.Model.Promotions;
using VirtoCommerce.Platform.Core.Events;

namespace VirtoCommerce.MarketingModule.Core.Events
{
    public class PromotionChangedEvent : GenericChangedEntryEvent<Promotion>
    {
        public PromotionChangedEvent(IEnumerable<GenericChangedEntry<Promotion>> changedEntries) : base(changedEntries)
        {
        }
    }
}
