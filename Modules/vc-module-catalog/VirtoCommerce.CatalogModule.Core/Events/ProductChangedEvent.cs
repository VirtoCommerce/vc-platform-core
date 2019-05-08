using System.Collections.Generic;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.Platform.Core.Events;

namespace VirtoCommerce.CatalogModule.Core.Events
{
    public class ProductChangedEvent : ChangesLogEvent<CatalogProduct>
    {
        public ProductChangedEvent(IEnumerable<GenericChangedEntry<CatalogProduct>> changedEntries)
            : base(changedEntries)
        {
        }
    }
}
