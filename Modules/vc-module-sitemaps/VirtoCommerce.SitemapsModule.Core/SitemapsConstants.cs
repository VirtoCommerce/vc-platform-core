using System;
using System.Collections.Generic;
using System.Text;
using VirtoCommerce.Platform.Core.Modularity;

namespace VirtoCommerce.SitemapsModule.Core
{
    public static class SitemapsConstants
    {
        public static class Security {
            public static class Permissions {
                public const string Read = "sitemaps:read",
                    Access = "sitemaps:access",
                    Create = "sitemaps:create",
                    Delete = "sitemaps:delete",
                    Update = "sitemaps:update";

                public static string[] AllPermissions = new[] { Access, Create, Delete, Update, Read };
            }
        }
        public static class Settings
        {
            public static class General
            {
                public static ModuleSetting RecordsLimitPerFile = new ModuleSetting
                {
                    Name = "Sitemap.RecordsLimitPerFile",
                    ValueType = ModuleSetting.TypeInteger,
                    DefaultValue = "10000"
                };

                public static ModuleSetting FilenameSeparator = new ModuleSetting
                {
                    Name = "Sitemap.FilenameSeparator",
                    ValueType = ModuleSetting.TypeString,
                    DefaultValue = "--"
                };

                public static ModuleSetting SearchBunchSize = new ModuleSetting
                {
                    Name = "Sitemap.SearchBunchSize",
                    ValueType = ModuleSetting.TypeInteger,
                    DefaultValue = "500"
                };

                public static ModuleSetting AcceptedFilenameExtensions = new ModuleSetting
                {
                    Name = "Sitemap.AcceptedFilenameExtensions",
                    ValueType = ModuleSetting.TypeString,
                    DefaultValue = ".md,.html"
                };

                public static IEnumerable<ModuleSetting> AllSettings
                {
                    get
                    {
                        yield return RecordsLimitPerFile;
                        yield return FilenameSeparator;
                        yield return SearchBunchSize;
                        yield return AcceptedFilenameExtensions;
                    }
                }
            }

            public static class ProductLinks
            {
                public static ModuleSetting ProductPageUpdateFrequency = new ModuleSetting
                {
                    Name = "Sitemap.ProductPageUpdateFrequency",
                    ValueType = ModuleSetting.TypeString,
                    IsArray = true,
                    ArrayValues = new string[] { "always", "hourly", "daily", "weekly", "monthly", "yearly", "never" },
                    DefaultValue = "daily"
                };

                public static ModuleSetting ProductPagePriority = new ModuleSetting
                {
                    Name = "Sitemap.ProductPagePriority",
                    ValueType = ModuleSetting.TypeDecimal,
                    DefaultValue = "1.0"
                };

                public static IEnumerable<ModuleSetting> AllSettings
                {
                    get
                    {
                        yield return ProductPageUpdateFrequency;
                        yield return ProductPagePriority;
                    }
                }
            }

            public static class CategoryLinks
            {
                public static ModuleSetting CategoryPageUpdateFrequency = new ModuleSetting
                {
                    Name = "Sitemap.CategoryPageUpdateFrequency",
                    ValueType = ModuleSetting.TypeString,
                    IsArray = true,
                    ArrayValues = new string[] { "always", "hourly", "daily", "weekly", "monthly", "yearly", "never" },
                    DefaultValue = "weekly"
                };

                public static ModuleSetting CategoryPagePriority = new ModuleSetting
                {
                   
                    Name = "Sitemap.CategoryPagePriority",
                    ValueType = ModuleSetting.TypeDecimal,
                    DefaultValue = "0.7"
                };

                public static IEnumerable<ModuleSetting> AllSettings
                {
                    get
                    {
                        yield return CategoryPageUpdateFrequency;
                        yield return CategoryPagePriority;
                    }
                }

            }
        }
    }
}
