using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Events;

namespace VirtoCommerce.CatalogModule.Core2.Events
{
    public class ProductChangingEvent : GenericChangedEntryEvent<Model.CatalogProduct>
    {
        public ProductChangingEvent(IEnumerable<GenericChangedEntry<Model.CatalogProduct>> changedEntries)
            : base(changedEntries)
        {
        }
    }
}
