using System.Collections.Generic;
using VirtoCommerce.CoreModule.Core.Model;
using VirtoCommerce.Platform.Core.Events;

namespace VirtoCommerce.CoreModule.Core.Events
{
    public class SeoInfoChangedEvent : GenericChangedEntryEvent<SeoInfo>
    {
        public SeoInfoChangedEvent(IEnumerable<GenericChangedEntry<SeoInfo>> changedEntries) : base(changedEntries)
        {
        }
    }
}
