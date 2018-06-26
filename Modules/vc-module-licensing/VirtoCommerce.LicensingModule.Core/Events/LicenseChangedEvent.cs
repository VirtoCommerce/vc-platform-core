using System.Collections.Generic;
using VirtoCommerce.LicensingModule.Core.Model;
using VirtoCommerce.Platform.Core.Events;

namespace VirtoCommerce.LicensingModule.Core.Events
{
    public class LicenseChangedEvent : GenericChangedEntryEvent<License>
    {
        public LicenseChangedEvent(IEnumerable<GenericChangedEntry<License>> changedEntries)
            : base(changedEntries)
        {
        }
    }
}
