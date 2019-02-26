using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Events;

namespace VirtoCommerce.CatalogModule.Core2.Events
{
    public class CatalogChangingEvent : GenericChangedEntryEvent<Model.Catalog>
    {
        public CatalogChangingEvent(IEnumerable<GenericChangedEntry<Model.Catalog>> changedEntries)
          : base(changedEntries)
        {
        }
    }
}
