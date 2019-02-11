using System.Collections.Generic;
using VirtoCommerce.MarketingModule.Core.Model.Promotions;
using VirtoCommerce.Platform.Core.Events;

namespace VirtoCommerce.MarketingModule.Core.Events
{
    public class PromotionUsageChangedEvent : GenericChangedEntryEvent<PromotionUsage>
    {
        public PromotionUsageChangedEvent(IEnumerable<GenericChangedEntry<PromotionUsage>> changedEntries) : base(changedEntries)
        {
        }
    }
}
