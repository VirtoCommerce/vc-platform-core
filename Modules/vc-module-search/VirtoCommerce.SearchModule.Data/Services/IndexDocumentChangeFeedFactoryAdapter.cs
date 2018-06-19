using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using VirtoCommerce.SearchModule.Core.Model;
using VirtoCommerce.SearchModule.Core.Services;

namespace VirtoCommerce.SearchModule.Data.Services
{
    /// <summary>
    /// Implements IIndexDocumentChangeFeedFactory on top of the older IIndexDocumentChangesProvider
    /// to support backwards compatibility.
    /// </summary>
    public class IndexDocumentChangeFeedFactoryAdapter : IIndexDocumentChangeFeedFactory
    {
        protected IIndexDocumentChangesProvider Provider { get; }

        public IndexDocumentChangeFeedFactoryAdapter(IIndexDocumentChangesProvider provider)
        {
            Provider = provider ?? throw new ArgumentNullException(nameof(provider));
        }

        public virtual async Task<IIndexDocumentChangeFeed> CreateFeed(DateTime? startDate, DateTime? endDate, int batchSize)
        {
            var totalCount = await Provider.GetTotalChangesCountAsync(startDate, endDate);
            return new IndexDocumentChangeFeedForProvider(Provider, startDate, endDate, totalCount, batchSize);
        }

        protected class IndexDocumentChangeFeedForProvider : IIndexDocumentChangeFeed
        {
            protected IIndexDocumentChangesProvider Provider { get; }
            protected DateTime? StartDate { get; }
            protected DateTime? EndDate { get; }

            public long? TotalCount { get; set; }
            protected long Skip { get; set; }
            protected long Take { get; }

            public IndexDocumentChangeFeedForProvider(IIndexDocumentChangesProvider provider,
                DateTime? startDate, DateTime? endDate, long totalCount, long take)
            {
                Provider = provider ?? throw new ArgumentNullException(nameof(provider));
                StartDate = startDate;
                EndDate = endDate;
                TotalCount = totalCount;
                Take = take;
            }

            public async Task<IReadOnlyCollection<IndexDocumentChange>> GetNextBatch()
            {
                if (Skip >= TotalCount)
                {
                    return await Task.FromResult<IReadOnlyCollection<IndexDocumentChange>>(null);
                }

                var changes = await Provider.GetChangesAsync(StartDate, EndDate, Skip, Take);
                if (changes.Count == 0)
                {
                    return await Task.FromResult<IReadOnlyCollection<IndexDocumentChange>>(null);
                }

                Skip += changes.Count;

                return new ReadOnlyCollection<IndexDocumentChange>(changes);
            }
        }
    }
}
