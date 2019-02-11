using System.Collections.Generic;
using VirtoCommerce.MarketingModule.Core.Model;
using VirtoCommerce.Platform.Core.Events;

namespace VirtoCommerce.MarketingModule.Core.Events
{
    public class DynamicContentItemChangedEvent : GenericChangedEntryEvent<DynamicContentItem>
    {
        public DynamicContentItemChangedEvent(IEnumerable<GenericChangedEntry<DynamicContentItem>> changedEntries) : base(changedEntries)
        {
        }
    }
}
