using System.Collections.Generic;
using System.Threading.Tasks;
using VirtoCommerce.SearchModule.Core.Model;

namespace VirtoCommerce.SearchModule.Core.Services
{
    /// <summary>
    /// Statefull change feed for indexation.
    /// Storage sub systems should be able to implement this effeciently, whether it be Sql or NoSql.
    /// </summary>
    public interface IIndexDocumentChangeFeed
    {
        /// <summary>
        /// Optional total count, feed is not required to implement this.
        /// This is only informational so that the user might have an idea how long the process will still take.
        /// </summary>
        long? TotalCount { get; }

        /// <summary>
        /// Gets the next batch with changes.
        /// </summary>
        /// <returns>Batch of changes or null when at end of feed.</returns>
        Task<IReadOnlyCollection<IndexDocumentChange>> GetNextBatch();
    }
}
