using System.Collections.Generic;
using System.Threading.Tasks;
using VirtoCommerce.SearchModule.Core.Model;

namespace VirtoCommerce.SearchModule.Core.Services
{
    public interface ISearchProvider
    {
        Task DeleteIndexAsync(string documentType);
        Task<IndexingResult> IndexAsync(string documentType, IList<IndexDocument> documents);
        Task<IndexingResult> RemoveAsync(string documentType, IList<IndexDocument> documents);
        Task<SearchResponse> SearchAsync(string documentType, SearchRequest request);
    }
}
