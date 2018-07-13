using System.Collections.Generic;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.Platform.Core.Events;

namespace VirtoCommerce.CartModule.Core.Events
{
    public class CartChangedEvent : GenericChangedEntryEvent<ShoppingCart>
    {       
        public CartChangedEvent(IEnumerable<GenericChangedEntry<ShoppingCart>> changedEntries)
          : base(changedEntries)
        {
        }      
    }
}
