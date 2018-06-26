using System.Collections.Generic;
using System.Threading.Tasks;
using VirtoCommerce.SearchModule.Core.Model;

namespace VirtoCommerce.SearchModule.Core.Services
{
    /// <summary>
    /// Used by indexing manager to get documents to be indexed
    /// </summary>
    public interface IIndexDocumentBuilder
    {
        Task<IList<IndexDocument>> GetDocumentsAsync(IList<string> documentIds);
    }
}
