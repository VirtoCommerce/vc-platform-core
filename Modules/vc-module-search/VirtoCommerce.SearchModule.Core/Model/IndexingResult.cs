using System.Collections.Generic;
using System.Diagnostics;

namespace VirtoCommerce.SearchModule.Core.Model
{
    /// <summary>
    /// Describes the result of the indexing operation for a batch of documents
    /// </summary>
    public class IndexingResult
    {
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public IList<IndexingResultItem> Items { get; set; }
    }
}
