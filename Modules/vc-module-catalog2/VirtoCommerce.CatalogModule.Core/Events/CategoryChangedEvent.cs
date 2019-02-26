using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Domain;
using VirtoCommerce.Platform.Core.Events;

namespace VirtoCommerce.CatalogModule.Core.Events
{
    public class CategoryChangedEvent : GenericChangedEntryEvent<Model.Category>
    {
        public CategoryChangedEvent(IEnumerable<GenericChangedEntry<Model.Category>> changedEntries)
            : base(changedEntries)
        {
        }
    }
}

