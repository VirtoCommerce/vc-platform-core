using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Events;

namespace VirtoCommerce.CatalogModule.Core2.Events
{
    public class PropertyChangingEvent : GenericChangedEntryEvent<Model.Property>
    {
        public PropertyChangingEvent(IEnumerable<GenericChangedEntry<Model.Property>> changedEntries)
            : base(changedEntries)
        {
        }
    }
}
