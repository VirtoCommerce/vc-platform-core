using System.Collections.Generic;

namespace VirtoCommerce.Domain.Search
{
    public class IndexDocumentConfiguration
    {
        public string DocumentType { get; set; }
        public IndexDocumentSource DocumentSource { get; set; }
        public IList<IndexDocumentSource> RelatedSources { get; set; }
    }
}
