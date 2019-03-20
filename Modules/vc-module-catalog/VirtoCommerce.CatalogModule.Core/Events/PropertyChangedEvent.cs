using System.Collections.Generic;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.Platform.Core.Events;

namespace VirtoCommerce.CatalogModule.Core.Events
{
    public class PropertyChangedEvent : GenericChangedEntryEvent<Property>
    {
        public PropertyChangedEvent(IEnumerable<GenericChangedEntry<Property>> changedEntries)
            : base(changedEntries)
        {
        }
    }
}
