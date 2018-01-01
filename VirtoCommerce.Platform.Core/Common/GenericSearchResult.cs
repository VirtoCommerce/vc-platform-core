using System;
using System.Collections.Generic;
using System.Text;

namespace VirtoCommerce.Platform.Core.Common
{
    public class GenericSearchResult<T>
    {
        public GenericSearchResult()
        {
            Results = new List<T>();
        }
        public int TotalCount { get; set; }
        public ICollection<T> Results { get; set; }
    }
}
