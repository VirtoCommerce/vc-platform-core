using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Domain;
using VirtoCommerce.Platform.Core.Events;

namespace VirtoCommerce.CatalogModule.Core.Events
{
    public class ProductChangingEvent : GenericChangedEntryEvent<Model.CatalogProduct>
    {
        public ProductChangingEvent(IEnumerable<GenericChangedEntry<Model.CatalogProduct>> changedEntries)
            : base(changedEntries)
        {
        }
    }
}
