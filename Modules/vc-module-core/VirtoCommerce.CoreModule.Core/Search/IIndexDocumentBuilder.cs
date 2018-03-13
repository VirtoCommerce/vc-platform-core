using System.Collections.Generic;
using System.Threading.Tasks;

namespace VirtoCommerce.Domain.Search
{
    /// <summary>
    /// Used by indexing manager to get documents to be indexed
    /// </summary>
    public interface IIndexDocumentBuilder
    {
        Task<IList<IndexDocument>> GetDocumentsAsync(IList<string> documentIds);
    }
}
