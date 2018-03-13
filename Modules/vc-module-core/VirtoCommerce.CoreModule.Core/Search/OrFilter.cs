using System.Collections.Generic;

namespace VirtoCommerce.Domain.Search
{
    public class OrFilter : IFilter
    {
        public IList<IFilter> ChildFilters { get; set; }

        public override string ToString()
        {
            return ChildFilters != null ? $"({string.Join(" OR ", ChildFilters)})" : string.Empty;
        }
    }
}
