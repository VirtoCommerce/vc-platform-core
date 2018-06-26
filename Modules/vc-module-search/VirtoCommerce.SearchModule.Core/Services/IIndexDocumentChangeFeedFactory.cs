using System;
using System.Threading.Tasks;

namespace VirtoCommerce.SearchModule.Core.Services
{
    /// <summary>
    /// Allows creating a statefull change feed as a source for the indexation process.
    /// </summary>
    public interface IIndexDocumentChangeFeedFactory
    {
        /// <summary>
        /// Creates the change feed.
        /// </summary>
        /// <param name="startDate">Start date as a filter for the changes.</param>
        /// <param name="endDate">End date as a filter for the changes.</param>
        /// <param name="batchSize">Size of the batches to use.</param>
        /// <returns>Created feed, never null.</returns>
        Task<IIndexDocumentChangeFeed> CreateFeed(DateTime? startDate, DateTime? endDate, int batchSize);
    }
}
