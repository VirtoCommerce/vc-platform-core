using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Events;

namespace VirtoCommerce.CatalogModule.Core2.Events
{
    public class CategoryChangedEvent : GenericChangedEntryEvent<Model.Category>
    {
        public CategoryChangedEvent(IEnumerable<GenericChangedEntry<Model.Category>> changedEntries)
            : base(changedEntries)
        {
        }
    }
}

