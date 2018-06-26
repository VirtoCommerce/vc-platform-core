using System;

namespace VirtoCommerce.SearchModule.Core.Model
{
    public class IndexState
    {
        public string Provider { get; set;  }

        public string Scope { get; set; }

        public string DocumentType { get; set; }

        public long? IndexedDocumentsCount { get; set; }

        public DateTime? LastIndexationDate { get; set; }
    }
}
