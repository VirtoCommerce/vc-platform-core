using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Domain;
using VirtoCommerce.Platform.Core.Events;

namespace VirtoCommerce.CatalogModule.Core.Events
{
    /// <summary>
    /// Event generated after catalogs entities were change
    /// </summary>
    public class GenericCatalogChangedEvent<T> : ChangedEntryEvent<T>
    {
        public GenericCatalogChangedEvent(IEnumerable<ChangedEntry<T>> changedEntries)
            : base(changedEntries)
        {         
        }
    }
}
