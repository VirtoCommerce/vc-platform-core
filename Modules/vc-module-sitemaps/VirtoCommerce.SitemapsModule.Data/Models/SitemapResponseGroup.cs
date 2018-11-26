using System;

namespace VirtoCommerce.SitemapsModule.Data.Models
{
    [Flags]
    public enum SitemapResponseGroup
    {
        Default = 0,
        WithItems = 1 << 0,
        Full = Default | WithItems
    }
}
