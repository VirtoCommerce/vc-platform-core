using VirtoCommerce.Domain.Search.ChangeFeed;

namespace VirtoCommerce.Domain.Search
{
    public class IndexDocumentSource
    {
        public IIndexDocumentBuilder DocumentBuilder { get; set; }

        /// <summary>
        /// Older paged abstraction to get changes, this still works although it's quite inefficient.
        /// </summary>
        public IIndexDocumentChangesProvider ChangesProvider { get; set; }

        /// <summary>
        /// Newer statefull feed to get changes.
        /// If this one is present, the changes provider will be ignored.
        /// </summary>
        public IIndexDocumentChangeFeedFactory ChangeFeedFactory { get; set; }
    }
}
