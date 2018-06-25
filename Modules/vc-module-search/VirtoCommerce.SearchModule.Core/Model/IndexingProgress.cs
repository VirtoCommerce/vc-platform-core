using System.Collections.Generic;
using System.Diagnostics;

namespace VirtoCommerce.SearchModule.Core.Model
{
    [DebuggerDisplay("{DocumentType}: {Description}")]
    public class IndexingProgress
    {
        public IndexingProgress(string description, string documentType, long? totalCount = null, long? processedCount = null, IList<string> errors = null)
        {
            Description = description;
            DocumentType = documentType;
            TotalCount = totalCount;
            ProcessedCount = processedCount;
            Errors = errors;
        }

        public string Description { get; set; }
        public string DocumentType { get; set; }
        public long? TotalCount { get; set; }
        public long? ProcessedCount { get; set; }
        public IList<string> Errors { get; set; }
        public long ErrorsCount => Errors?.Count ?? 0;
    }
}
