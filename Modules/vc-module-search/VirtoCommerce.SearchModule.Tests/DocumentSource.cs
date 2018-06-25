using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.SearchModule.Core.Model;
using VirtoCommerce.SearchModule.Core.Services;

namespace VirtoCommerce.SearchModule.Tests
{
    public class DocumentSource : IIndexDocumentChangesProvider, IIndexDocumentBuilder
    {
        public DocumentSource(string name)
        {
            Name = name;
        }

        public string Name { get; }
        public IList<string> DocumentIds { get; set; }
        public IList<IndexDocumentChange> Changes { get; set; }

        public virtual Task<long> GetTotalChangesCountAsync(DateTime? startDate, DateTime? endDate)
        {
            long result;

            if (startDate == null && endDate == null)
            {
                result = DocumentIds?.Count ?? 0;
            }
            else
            {
                result = GetChangesQuery(startDate, endDate).Count();
            }

            return Task.FromResult(result);
        }

        public virtual Task<IList<IndexDocumentChange>> GetChangesAsync(DateTime? startDate, DateTime? endDate, long skip, long take)
        {
            IList<IndexDocumentChange> result;

            if (startDate == null && endDate == null)
            {
                result = DocumentIds.Select(id => new IndexDocumentChange
                {
                    DocumentId = id,
                    ChangeType = IndexDocumentChangeType.Modified,
                    ChangeDate = DateTime.UtcNow
                })
                .Skip((int)skip)
                .Take((int)take)
                .ToArray();
            }
            else
            {
                var changes = GetChangesQuery(startDate, endDate);
                result = changes.Skip((int)skip).Take((int)take).ToArray();
            }
            return Task.FromResult(result);
        }

        public virtual Task<IList<IndexDocument>> GetDocumentsAsync(IList<string> documentIds)
        {
            var validDocumentIds = DocumentIds?.Intersect(documentIds) ?? documentIds;
            IList<IndexDocument> result = validDocumentIds.Select(id => CreateDocument(id, Name)).ToArray();
            return Task.FromResult(result);
        }


        protected static IndexDocument CreateDocument(string id, string fieldName)
        {
            var result = new IndexDocument(id);
            result.Add(new IndexDocumentField(fieldName, id));
            return result;
        }

        protected virtual IQueryable<IndexDocumentChange> GetChangesQuery(DateTime? startDate, DateTime? endDate)
        {
            var result = Changes.AsQueryable();

            if (startDate != null)
            {
                result = result.Where(c => c.ChangeDate >= startDate.Value);
            }

            if (endDate != null)
            {
                result = result.Where(c => c.ChangeDate <= endDate.Value);
            }

            return result;
        }
    }
}
