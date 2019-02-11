using System.Collections.Generic;
using VirtoCommerce.MarketingModule.Core.Model;
using VirtoCommerce.Platform.Core.Events;

namespace VirtoCommerce.MarketingModule.Core.Events
{
    public class DynamicContentFolderChangedEvent : GenericChangedEntryEvent<DynamicContentFolder>
    {
        public DynamicContentFolderChangedEvent(IEnumerable<GenericChangedEntry<DynamicContentFolder>> changedEntries) : base(changedEntries)
        {
        }
    }
}
