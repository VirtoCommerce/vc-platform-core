using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Events;

namespace VirtoCommerce.Platform.Core.Security.Events
{
    public class UserChangedEvent : DomainEvent
    {
        public UserChangedEvent(GenericChangedEntry<ApplicationUser> changedEntry)
        {
            ChangedEntry = changedEntry;
        }

        public GenericChangedEntry<ApplicationUser> ChangedEntry { get; set; }
    }
}
