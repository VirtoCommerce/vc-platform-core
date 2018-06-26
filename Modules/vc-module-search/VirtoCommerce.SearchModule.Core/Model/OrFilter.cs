using System.Collections.Generic;

namespace VirtoCommerce.SearchModule.Core.Model
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
