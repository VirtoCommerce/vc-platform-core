using System;

namespace VirtoCommerce.Domain.Search
{
    public class IndexDocumentChange
    {
        public string DocumentId { get; set; }
        public DateTime ChangeDate { get; set; }
        public IndexDocumentChangeType ChangeType { get; set; }
    }
}
