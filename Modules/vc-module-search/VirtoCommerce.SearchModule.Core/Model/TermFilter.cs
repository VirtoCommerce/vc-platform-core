using System.Collections.Generic;

namespace VirtoCommerce.SearchModule.Core.Model
{
    public class TermFilter : IFilter
    {
        public string FieldName { get; set; }
        public IList<string> Values { get; set; }

        public override string ToString()
        {
            return $"{FieldName}:{string.Join(",", Values)}";
        }
    }
}
