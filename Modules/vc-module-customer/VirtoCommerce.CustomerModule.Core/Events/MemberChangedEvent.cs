using System.Collections.Generic;
using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.Platform.Core.Events;

namespace VirtoCommerce.CustomerModule.Core.Events
{
    public class MemberChangedEvent : GenericChangedEntryEvent<Member>
    {
        public MemberChangedEvent(IEnumerable<GenericChangedEntry<Member>> changedEntries)
            : base(changedEntries)
        {
        }
    }
}
