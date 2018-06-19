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
    /// Change feed for an specific set of documents.
    /// This allows us to build all indexation logic on top of a change feed.
    /// </summary>
    public class InMemoryIndexDocumentChangeFeed : IIndexDocumentChangeFeed
    {
        protected string[] DocumentIds { get; }
        protected IndexDocumentChangeType ChangeType { get; }
        protected long Skip { get; set; }
        protected long Take { get; }

        public long? TotalCount { get; }

        public InMemoryIndexDocumentChangeFeed(string[] documentIds, IndexDocumentChangeType changeType, long take)
        {
            DocumentIds = documentIds ?? throw new ArgumentNullException(nameof(documentIds));
            ChangeType = changeType;
            TotalCount = documentIds.Length;
            Take = take;
        }
        public Task<IReadOnlyCollection<IndexDocumentChange>> GetNextBatch()
        {
            if (Skip >= TotalCount)
            {
                return Task.FromResult<IReadOnlyCollection<IndexDocumentChange>>(null);
            }

            var changes = DocumentIds
                .Skip((int) Skip)
                .Take((int)Take)
                .Select(x => new IndexDocumentChange
                {
                    DocumentId = x,
                    ChangeDate = DateTime.UtcNow,
                    ChangeType = ChangeType
                })
                .ToList();

            Skip += changes.Count;

            return Task.FromResult((IReadOnlyCollection<IndexDocumentChange>)
                new ReadOnlyCollection<IndexDocumentChange>(changes));
        }
    }
}
