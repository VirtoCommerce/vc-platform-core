using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
        private readonly IIndexDocumentChangesProvider _provider;

        public IndexDocumentChangeFeedFactoryAdapter(IIndexDocumentChangesProvider provider)
        {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
        }

        public virtual async Task<IIndexDocumentChangeFeed> CreateFeed(DateTime? startDate, DateTime? endDate, int batchSize)
        {
            var totalCount = await _provider.GetTotalChangesCountAsync(startDate, endDate);
            return new IndexDocumentChangeFeedForProvider(_provider, startDate, endDate, totalCount, batchSize);
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
                IReadOnlyCollection<IndexDocumentChange> result = null;

                if (Skip < TotalCount)
                {
                    var changes = await Provider.GetChangesAsync(StartDate, EndDate, Skip, Take);
                    if (changes.Any())
                    {
                        result = new ReadOnlyCollection<IndexDocumentChange>(changes);
                        Skip += changes.Count;
                    }
                }

                return result;
            }
        }
    }
}
