using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ExportModule.Core.Model
{
    public class ExportProgressInfo : ValueObject
    {
        public string[] Errors { get; set; }
        public int ProcessedCount { get; set; }
        public int TotalCount { get; set; }
        public int Description { get; set; }
    }
}
