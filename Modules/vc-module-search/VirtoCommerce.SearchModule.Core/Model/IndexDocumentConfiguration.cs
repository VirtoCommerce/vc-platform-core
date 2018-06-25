using System.Collections.Generic;

namespace VirtoCommerce.SearchModule.Core.Model
{
    public class IndexDocumentConfiguration
    {
        public string DocumentType { get; set; }
        public IndexDocumentSource DocumentSource { get; set; }
        public IList<IndexDocumentSource> RelatedSources { get; set; }
    }
}
