using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.Platform.Core.Domain
{
    public class GenericChangedEntry<T> : ValueObject
    {
        public GenericChangedEntry(T entry, EntryState state)
            : this(entry, entry, state)
        {
        }

        public GenericChangedEntry(T newEntry, T oldEntry, EntryState state)
        {
            NewEntry = newEntry;
            OldEntry = oldEntry;
            EntryState = state;
        }

        public EntryState EntryState { get; set; }
        public T NewEntry { get; set; }
        public T OldEntry { get; set; }
    }
}
