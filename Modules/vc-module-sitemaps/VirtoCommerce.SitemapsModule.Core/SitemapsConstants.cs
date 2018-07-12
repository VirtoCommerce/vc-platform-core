using System;
using System.Collections.Generic;
using System.Text;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Core.Settings;

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
                public static SettingDescriptor RecordsLimitPerFile = new SettingDescriptor
                {
                    Name = "Sitemap.RecordsLimitPerFile",
                    ValueType = SettingValueType.Integer,
                    DefaultValue = "10000"
                };

                public static SettingDescriptor FilenameSeparator = new SettingDescriptor
                {
                    Name = "Sitemap.FilenameSeparator",
                    ValueType = SettingValueType.ShortText,
                    DefaultValue = "--"
                };

                public static SettingDescriptor SearchBunchSize = new SettingDescriptor
                {
                    Name = "Sitemap.SearchBunchSize",
                    ValueType = SettingValueType.Integer,
                    DefaultValue = "500"
                };

                public static SettingDescriptor AcceptedFilenameExtensions = new SettingDescriptor
                {
                    Name = "Sitemap.AcceptedFilenameExtensions",
                    ValueType = SettingValueType.ShortText,
                    DefaultValue = ".md,.html"
                };

                public static IEnumerable<SettingDescriptor> AllSettings
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
                public static SettingDescriptor ProductPageUpdateFrequency = new SettingDescriptor
                {
                    Name = "Sitemap.ProductPageUpdateFrequency",
                    ValueType = SettingValueType.ShortText,
                    IsDictionary = true,
                    AllowedValues = new string[] { "always", "hourly", "daily", "weekly", "monthly", "yearly", "never" },
                    DefaultValue = "daily"
                };

                public static SettingDescriptor ProductPagePriority = new SettingDescriptor
                {
                    Name = "Sitemap.ProductPagePriority",
                    ValueType = SettingValueType.Decimal,
                    DefaultValue = "1.0"
                };

                public static IEnumerable<SettingDescriptor> AllSettings
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
                public static SettingDescriptor CategoryPageUpdateFrequency = new SettingDescriptor
                {
                    Name = "Sitemap.CategoryPageUpdateFrequency",
                    ValueType = SettingValueType.ShortText,
                    IsDictionary = true,
                    AllowedValues = new string[] { "always", "hourly", "daily", "weekly", "monthly", "yearly", "never" },
                    DefaultValue = "weekly"
                };

                public static SettingDescriptor CategoryPagePriority = new SettingDescriptor
                {
                   
                    Name = "Sitemap.CategoryPagePriority",
                    ValueType = SettingValueType.Decimal,
                    DefaultValue = "0.7"
                };

                public static IEnumerable<SettingDescriptor> AllSettings
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
