using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.SearchModule.Core.Model;
using VirtoCommerce.SearchModule.Core.Services;

namespace VirtoCommerce.SearchModule.Tests
{
    public class SearchProvider : ISearchProvider
    {
        public IDictionary<string, IndexDocument> IndexedDocuments { get; } = new Dictionary<string, IndexDocument>();

        public Task DeleteIndexAsync(string documentType)
        {
            return Task.FromResult<object>(null);
        }

        public Task<IndexingResult> IndexAsync(string documentType, IList<IndexDocument> documents)
        {
            foreach (var document in documents.Where(d => d.Id.StartsWith("good")))
            {
                IndexedDocuments[document.Id] = document;
            }

            var result = GetIndexingResult(documents);
            return Task.FromResult(result);
        }

        public Task<IndexingResult> RemoveAsync(string documentType, IList<IndexDocument> documents)
        {
            foreach (var document in documents)
            {
                if (IndexedDocuments.ContainsKey(document.Id))
                {
                    IndexedDocuments.Remove(document.Id);
                }
            }

            var result = GetIndexingResult(documents);
            return Task.FromResult(result);
        }

        public Task<SearchResponse> SearchAsync(string documentType, SearchRequest request)
        {
            throw new NotImplementedException();
        }


        private static IndexingResult GetIndexingResult(IEnumerable<IndexDocument> documents)
        {
            return new IndexingResult
            {
                Items = documents.Select(GetIndexingResult).ToArray()
            };
        }

        private static IndexingResultItem GetIndexingResult(IndexDocument document)
        {
            var error = document.Id.StartsWith("bad");

            return new IndexingResultItem
            {
                Id = document.Id,
                Succeeded = !error,
                ErrorMessage = error ? "Search provider error" : null,
            };
        }
    }
}
