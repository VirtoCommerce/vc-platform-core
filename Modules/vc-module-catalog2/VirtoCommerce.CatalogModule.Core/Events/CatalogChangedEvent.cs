using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Events;

namespace VirtoCommerce.CatalogModule.Core2.Events
{
    public class CatalogChangedEvent : GenericChangedEntryEvent<Model.Catalog>
    {
        public CatalogChangedEvent(IEnumerable<GenericChangedEntry<Model.Catalog>> changedEntries)
            : base(changedEntries)
        {
        }
    }
}
