using System.Collections.Generic;

namespace VirtoCommerce.SearchModule.Core.Model
{
    public class AndFilter : IFilter
    {
        public IList<IFilter> ChildFilters { get; set; }

        public override string ToString()
        {
            return ChildFilters != null ? $"({string.Join(" AND ", ChildFilters)})" : string.Empty;
        }
    }
}
