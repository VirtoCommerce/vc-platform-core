using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Domain;
using VirtoCommerce.Platform.Core.Events;

namespace VirtoCommerce.CatalogModule.Core.Events
{
    public class PropertyChangingEvent : GenericChangedEntryEvent<Model.Property>
    {
        public PropertyChangingEvent(IEnumerable<GenericChangedEntry<Model.Property>> changedEntries)
            : base(changedEntries)
        {
        }
    }
}
