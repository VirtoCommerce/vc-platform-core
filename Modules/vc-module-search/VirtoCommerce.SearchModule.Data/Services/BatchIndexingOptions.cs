using System.Collections.Generic;
using VirtoCommerce.SearchModule.Core.Services;

namespace VirtoCommerce.SearchModule.Data.Services
{
    public class BatchIndexingOptions
    {
        public string DocumentType { get; set; }
        public IIndexDocumentBuilder PrimaryDocumentBuilder { get; set; }
        public IList<IIndexDocumentBuilder> SecondaryDocumentBuilders { get; set; }
    }
}
