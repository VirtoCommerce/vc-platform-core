using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Domain;
using VirtoCommerce.Platform.Core.Events;

namespace VirtoCommerce.CatalogModule.Core.Events
{
    public class PropertyChangedEvent : GenericChangedEntryEvent<Model.Property>
    {
        public PropertyChangedEvent(IEnumerable<GenericChangedEntry<Model.Property>> changedEntries)
            : base(changedEntries)
        {
        }
    }
}
