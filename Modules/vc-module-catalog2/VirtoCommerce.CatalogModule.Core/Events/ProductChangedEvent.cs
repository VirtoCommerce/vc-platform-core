using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Events;

namespace VirtoCommerce.CatalogModule.Core2.Events
{
    public class ProductChangedEvent : GenericChangedEntryEvent<Model.CatalogProduct>
    {
        public ProductChangedEvent(IEnumerable<GenericChangedEntry<Model.CatalogProduct>> changedEntries)
            : base(changedEntries)
        {
        }
    }
}
