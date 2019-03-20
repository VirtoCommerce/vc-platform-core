using System.Collections.Generic;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.Platform.Core.Events;

namespace VirtoCommerce.CatalogModule.Core.Events
{
    public class CategoryChangingEvent : GenericChangedEntryEvent<Category>
    {
        public CategoryChangingEvent(IEnumerable<GenericChangedEntry<Category>> changedEntries)
            : base(changedEntries)
        {
        }
    }
}
