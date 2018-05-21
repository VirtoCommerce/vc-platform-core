using System.Collections.Generic;

namespace VirtoCommerce.Domain.Search
{
    public class RangeFilter : IFilter
    {
        public string FieldName { get; set; }
        public IList<RangeFilterValue> Values { get; set; }

        public override string ToString()
        {
            return FieldName != null && Values != null ? $"{FieldName}:{string.Join(",", Values)}" : string.Empty;
        }
    }
}
