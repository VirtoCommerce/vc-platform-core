using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ExportModule.Core.Model
{
    public class ExportProgressInfo : ValueObject
    {
        public ExportProgressInfo()
        {
            Errors = new List<string>();
        }

        public string Description { get; set; }
        public List<string> Errors { get; set; }
        public int ProcessedCount { get; set; }
        public int TotalCount { get; set; }
        public long ErrorCount
        {
            get
            {
                return Errors?.Count ?? 0;
            }
        }
    }
}
