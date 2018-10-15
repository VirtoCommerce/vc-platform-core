using System.Collections.Generic;

namespace VirtoCommerce.PricingModule.Core.Model.Search
{
    public class PricingSearchResult<T>
    {
        public int TotalCount { get; set; }
        public ICollection<T> Results { get; set; }

    }
}
