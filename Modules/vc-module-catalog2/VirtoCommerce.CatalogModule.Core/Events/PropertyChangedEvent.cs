using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Events;

namespace VirtoCommerce.CatalogModule.Core2.Events
{
    public class PropertyChangedEvent : GenericChangedEntryEvent<Model.Property>
    {
        public PropertyChangedEvent(IEnumerable<GenericChangedEntry<Model.Property>> changedEntries)
            : base(changedEntries)
        {
        }
    }
}
