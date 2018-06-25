using System;

namespace VirtoCommerce.SearchModule.Core.Model
{
    public class IndexDocumentChange
    {
        public string DocumentId { get; set; }
        public DateTime ChangeDate { get; set; }
        public IndexDocumentChangeType ChangeType { get; set; }
    }
}
