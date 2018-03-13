using System;
using System.Collections.Generic;

namespace VirtoCommerce.Domain.Search
{
    public class IndexingOptions
    {
        public string DocumentType { get; set; }
        public IList<string> DocumentIds { get; set; }
        public bool DeleteExistingIndex { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? BatchSize { get; set; }
    }
}
