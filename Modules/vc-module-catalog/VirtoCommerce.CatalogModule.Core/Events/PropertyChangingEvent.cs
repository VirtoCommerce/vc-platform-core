using System.Collections.Generic;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.Platform.Core.Events;

namespace VirtoCommerce.CatalogModule.Core.Events
{
    public class PropertyChangingEvent : GenericChangedEntryEvent<Property>
    {
        public PropertyChangingEvent(IEnumerable<GenericChangedEntry<Property>> changedEntries)
            : base(changedEntries)
        {
        }
    }
}
