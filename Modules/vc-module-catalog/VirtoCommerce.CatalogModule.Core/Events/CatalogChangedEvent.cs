using System.Collections.Generic;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.Platform.Core.Events;

namespace VirtoCommerce.CatalogModule.Core.Events
{
    public class CatalogChangedEvent : GenericChangedEntryEvent<Catalog>
    {
        public CatalogChangedEvent(IEnumerable<GenericChangedEntry<Catalog>> changedEntries)
            : base(changedEntries)
        {
        }
    }
}
