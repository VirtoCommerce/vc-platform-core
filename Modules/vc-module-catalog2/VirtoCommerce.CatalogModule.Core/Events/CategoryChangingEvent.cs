using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Domain;
using VirtoCommerce.Platform.Core.Events;

namespace VirtoCommerce.CatalogModule.Core.Events
{
    public class CategoryChangingEvent : GenericChangedEntryEvent<Model.Category>
    {
        public CategoryChangingEvent(IEnumerable<GenericChangedEntry<Model.Category>> changedEntries)
            : base(changedEntries)
        {
        }
    }
}
