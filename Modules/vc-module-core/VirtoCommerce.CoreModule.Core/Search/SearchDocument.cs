using System;
using System.Collections.Generic;

namespace VirtoCommerce.Domain.Search
{
    [Serializable]
    public class SearchDocument : Dictionary<string, object>
    {
        public string Id { get; set; }
    }
}
