using System;
using System.Collections.Generic;
using System.Text;

namespace VirtoCommerce.SitemapsModule.Core.Models
{
    public static class UrlTemplatePatterns
    {
        public const string StoreUrl = "{storeUrl}",
                            StoreSecureUrl = "{storeSecureUrl}",
                            Language = "{language}",
                            Slug = "{slug}";
    }
}
