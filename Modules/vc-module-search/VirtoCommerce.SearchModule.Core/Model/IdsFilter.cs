using System.Collections.Generic;

namespace VirtoCommerce.SearchModule.Core.Model
{
    public class IdsFilter : IFilter
    {
        public IList<string> Values { get; set; }

        public override string ToString()
        {
            return $"ID:{string.Join(",", Values)}";
        }
    }
}
