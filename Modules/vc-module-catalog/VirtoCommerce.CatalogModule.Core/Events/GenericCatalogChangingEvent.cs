using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Domain;
using VirtoCommerce.Platform.Core.Events;

namespace VirtoCommerce.CatalogModule.Core.Events
{
    /// <summary>
    /// Event generated before catalogs entities were change
    /// </summary>
    public class GenericCatalogChangingEvent<T> : ChangeEntryEvent<T>
    {
        public GenericCatalogChangingEvent(IEnumerable<ChangedEntry<T>> changedEntries)
            : base(changedEntries)
        {
        }
    }
}
