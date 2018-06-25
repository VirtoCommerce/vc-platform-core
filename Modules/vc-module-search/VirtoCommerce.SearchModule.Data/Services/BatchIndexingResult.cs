using VirtoCommerce.SearchModule.Core.Model;

namespace VirtoCommerce.SearchModule.Data.Services
{
    public class BatchIndexingResult
    {
        public IndexingResult IndexingResult { get; set; }
        public long ProcessedCount { get; set; }
    }
}
