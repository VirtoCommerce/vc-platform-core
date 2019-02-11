using System.Collections.Generic;
using VirtoCommerce.MarketingModule.Core.Model;
using VirtoCommerce.Platform.Core.Events;

namespace VirtoCommerce.MarketingModule.Core.Events
{
    public class DynamicContentPlaceChangedEvent : GenericChangedEntryEvent<DynamicContentPlace>
    {
        public DynamicContentPlaceChangedEvent(IEnumerable<GenericChangedEntry<DynamicContentPlace>> changedEntries) : base(changedEntries)
        {
        }
    }
}
