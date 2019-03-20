using System.Collections.Generic;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.Platform.Core.Events;

namespace VirtoCommerce.CatalogModule.Core.Events
{
    public class CategoryChangedEvent : GenericChangedEntryEvent<Category>
    {
        public CategoryChangedEvent(IEnumerable<GenericChangedEntry<Category>> changedEntries)
            : base(changedEntries)
        {
        }
    }
}
