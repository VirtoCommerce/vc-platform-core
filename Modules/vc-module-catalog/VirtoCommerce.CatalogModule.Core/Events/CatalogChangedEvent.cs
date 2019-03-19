using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Domain;
using VirtoCommerce.Platform.Core.Events;

namespace VirtoCommerce.CatalogModule.Core.Events
{
    public class CatalogChangedEvent : GenericChangedEntryEvent<Model.Catalog>
    {
        public CatalogChangedEvent(IEnumerable<GenericChangedEntry<Model.Catalog>> changedEntries)
            : base(changedEntries)
        {
        }
    }
}
