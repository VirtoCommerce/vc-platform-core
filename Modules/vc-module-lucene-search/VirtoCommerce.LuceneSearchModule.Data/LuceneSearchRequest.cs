using Lucene.Net.Search;

namespace VirtoCommerce.LuceneSearchModule.Data
{
    public class LuceneSearchRequest
    {
        public Query Query { get; set; }
        public Filter Filter { get; set; }
        public Sort Sort { get; set; }
        public int Count { get; set; }
    }
}
