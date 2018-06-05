using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Events;

namespace VirtoCommerce.Platform.Core.Security.Events
{
    public class UserChangingEvent : DomainEvent
    {
        public UserChangingEvent(GenericChangedEntry<ApplicationUser> changedEntry)
        {
            ChangedEntry = changedEntry;
        }

        public GenericChangedEntry<ApplicationUser> ChangedEntry { get; set; }
    }
}
