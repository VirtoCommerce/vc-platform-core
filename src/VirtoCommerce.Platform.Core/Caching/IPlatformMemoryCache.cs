using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Caching.Memory;

namespace VirtoCommerce.Platform.Core.Caching
{
    public interface IPlatformMemoryCache : IMemoryCache
    {
        void RemoveByPattern(string pattern);
    }
}
