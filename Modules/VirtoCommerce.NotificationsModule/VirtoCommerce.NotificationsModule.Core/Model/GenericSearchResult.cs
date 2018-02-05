using System;
using System.Collections.Generic;
using System.Text;

namespace VirtoCommerce.NotificationsModule.Core.Model
{
    public class GenericSearchResult<T>
    {
        public int TotalCount { get; set; }
        public ICollection<T> Results { get; set; }
    }
}
