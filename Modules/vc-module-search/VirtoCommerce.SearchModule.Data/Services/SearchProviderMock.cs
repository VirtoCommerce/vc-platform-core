using System.Collections.Generic;
using System.Threading.Tasks;
using VirtoCommerce.SearchModule.Core.Model;
using VirtoCommerce.SearchModule.Core.Services;

namespace VirtoCommerce.SearchModule.Data.Services
{
    public class SearchProviderMock : ISearchProvider
    {
        public Task DeleteIndexAsync(string documentType)
        {
            throw new System.NotImplementedException();
        }

        public Task<IndexingResult> IndexAsync(string documentType, IList<IndexDocument> documents)
        {
            throw new System.NotImplementedException();
        }

        public Task<IndexingResult> RemoveAsync(string documentType, IList<IndexDocument> documents)
        {
            throw new System.NotImplementedException();
        }

        public Task<SearchResponse> SearchAsync(string documentType, SearchRequest request)
        {
            throw new System.NotImplementedException();
        }
    }
}
