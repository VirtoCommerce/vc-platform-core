using System.Collections.Generic;
using VirtoCommerce.ContentModule.Core.Model;
using VirtoCommerce.Platform.Core.Events;

namespace VirtoCommerce.ContentModule.Core.Events
{
    public class MenuLinkListChangedEvent : GenericChangedEntryEvent<MenuLinkList>
    {
        public MenuLinkListChangedEvent(IEnumerable<GenericChangedEntry<MenuLinkList>> changedEntries) : base(changedEntries)
        {
        }
    }
}
