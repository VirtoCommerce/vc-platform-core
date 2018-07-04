using VirtoCommerce.StoreModule.Core.Model;

namespace VirtoCommerce.StoreModule.Web.Model
{
    public class SearchResult
    {
        public int TotalCount { get; set; }
        public Store[] Stores { get; set; }
    }
}
