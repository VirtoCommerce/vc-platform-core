using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Events;

namespace VirtoCommerce.CoreModule.Core.Events
{
    public class CurrencyChangedEvent : GenericChangedEntryEvent<Currency.Currency>
    {
        public CurrencyChangedEvent(IEnumerable<GenericChangedEntry<Currency.Currency>> changedEntries) : base(changedEntries)
        {
        }
    }
}
