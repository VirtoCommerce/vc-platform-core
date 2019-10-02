using System;
using System.Collections.Generic;
using System.Text;

namespace VirtoCommerce.Platform.Caching
{
    public class CachingOptions
    {
        public bool CacheEnabled { get; set; } = true;
        public string Provider { get; set; } = "Locally";

        public TimeSpan? CacheAbsoluteExpiration { get; set; }
        public TimeSpan? CacheSlidingExpiration { get; set; }
    }
}
