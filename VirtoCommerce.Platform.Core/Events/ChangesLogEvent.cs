using System.Collections.Generic;
using VirtoCommerce.Platform.Core.ChangeLog;

namespace VirtoCommerce.Platform.Core.Events
{
    public class ChangesLogEvent<T> : GenericChangedEntryEvent<T> where T : IChangesLog
    {
        public ChangesLogEvent(IEnumerable<GenericChangedEntry<T>> changedEntries) : base(changedEntries)
        {
        }
    }
}
