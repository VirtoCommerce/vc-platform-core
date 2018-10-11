using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Settings;

namespace VirtoCommerce.SitemapsModule.Core.ModuleConstants
{
    public static class ModuleSettings
    {
        public static readonly SettingDescriptor RecordsLimitPerFile = new SettingDescriptor
        {
            Name = "Sitemap.RecordsLimitPerFile",
            GroupName = "Sitemap|General",
            ValueType = SettingValueType.Integer,
            DefaultValue = 10000
        };

        public static readonly SettingDescriptor FilenameSeparator = new SettingDescriptor
        {
            Name = "Sitemap.FilenameSeparator",
            GroupName = "Sitemap|General",
            ValueType = SettingValueType.ShortText,
            DefaultValue = "--"
        };

        public static readonly SettingDescriptor SearchBunchSize = new SettingDescriptor
        {
            Name = "Sitemap.SearchBunchSize",
            GroupName = "Sitemap|General",
            ValueType = SettingValueType.Integer,
            DefaultValue = 500
        };

        public static readonly SettingDescriptor AcceptedFilenameExtensions = new SettingDescriptor
        {
            Name = "Sitemap.AcceptedFilenameExtensions",
            GroupName = "Sitemap|General",
            ValueType = SettingValueType.ShortText,
            DefaultValue = ".md,.html"
        };

        public static readonly SettingDescriptor ProductPageUpdateFrequency = new SettingDescriptor
        {
            Name = "Sitemap.ProductPageUpdateFrequency",
            GroupName = "Sitemap|Product Links",
            ValueType = SettingValueType.ShortText,
            IsDictionary = true,
            AllowedValues = new object[]
            {
                "always",
                "hourly",
                "daily",
                "weekly",
                "monthly",
                "yearly",
                "never"
            },
            DefaultValue = "daily"
        };

        public static readonly SettingDescriptor ProductPagePriority = new SettingDescriptor
        {
            Name = "Sitemap.ProductPagePriority",
            GroupName = "Sitemap|Product Links",
            ValueType = SettingValueType.Decimal,
            DefaultValue = 1.0
        };

        public static readonly SettingDescriptor CategoryPageUpdateFrequency = new SettingDescriptor
        {
            Name = "Sitemap.CategoryPageUpdateFrequency",
            GroupName = "Sitemap|Category Links",
            ValueType = SettingValueType.ShortText,
            IsDictionary = true,
            AllowedValues = new object[]
            {
                "always",
                "hourly",
                "daily",
                "weekly",
                "monthly",
                "yearly",
                "never"
            },
            DefaultValue = "weekly"
        };

        public static readonly SettingDescriptor CategoryPagePriority = new SettingDescriptor
        {
            Name = "Sitemap.CategoryPagePriority",
            GroupName = "Sitemap|Product Links",
            ValueType = SettingValueType.Decimal,
            DefaultValue = 0.7
        };

        public static IEnumerable<SettingDescriptor> AllSettings
        {
            get
            {
                yield return RecordsLimitPerFile;
                yield return FilenameSeparator;
                yield return SearchBunchSize;
                yield return AcceptedFilenameExtensions;
                yield return ProductPageUpdateFrequency;
                yield return ProductPagePriority;
                yield return CategoryPageUpdateFrequency;
                yield return CategoryPagePriority;
            }
        }
    }
}
