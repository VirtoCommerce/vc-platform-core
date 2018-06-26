using VirtoCommerce.SearchModule.Core.Model;

namespace VirtoCommerce.SearchModule.Core.Services
{
    /// <summary>
    /// Allows queueing indexation working.
    /// Can be a serious performance boost for full re-indexation.
    /// </summary>
    public interface IIndexingWorker
    {
        void IndexDocuments(string documentType, string[] documentIds, IndexingPriority priority = IndexingPriority.Default);

        void DeleteDocuments(string documentType, string[] documentIds, IndexingPriority priority = IndexingPriority.Default);
    }
}
