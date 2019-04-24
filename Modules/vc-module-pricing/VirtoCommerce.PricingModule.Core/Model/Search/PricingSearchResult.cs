using System;
using System.Collections.Generic;

namespace VirtoCommerce.PricingModule.Core.Model.Search
{
    [Obsolete("This class is not used by PricingModule anymore. Please use GenericSearchResult<T> instead.")]
    public class PricingSearchResult<T>
    {
        public int TotalCount { get; set; }
        public ICollection<T> Results { get; set; }
    }
}
