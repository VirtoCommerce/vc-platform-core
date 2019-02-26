using System.Collections.Generic;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.Platform.Core.Events;

namespace VirtoCommerce.CatalogModule.Core.Events
{
    public class ProductChangingEvent : GenericChangedEntryEvent<CatalogProduct>
    {
        public ProductChangingEvent(IEnumerable<GenericChangedEntry<CatalogProduct>> changedEntries)
            : base(changedEntries)
        {
        }
    }
}
