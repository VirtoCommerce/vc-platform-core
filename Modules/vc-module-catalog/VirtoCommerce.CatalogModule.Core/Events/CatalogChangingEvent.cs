using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Domain;
using VirtoCommerce.Platform.Core.Events;

namespace VirtoCommerce.CatalogModule.Core.Events
{
    public class CatalogChangingEvent : GenericChangedEntryEvent<Model.Catalog>
    {
        public CatalogChangingEvent(IEnumerable<GenericChangedEntry<Model.Catalog>> changedEntries)
          : base(changedEntries)
        {
        }
    }
}
