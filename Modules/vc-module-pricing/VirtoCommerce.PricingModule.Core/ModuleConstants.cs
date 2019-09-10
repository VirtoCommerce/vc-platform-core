using System;
using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Settings;

namespace VirtoCommerce.PricingModule.Core
{
    public static class ModuleConstants
    {
        public static class Security
        {
            public static class Permissions
            {
                public const string Read = "pricing:read";
                public const string Create = "pricing:create";
                public const string Access = "pricing:access";
                public const string Update = "pricing:update";
                public const string Delete = "pricing:delete";
                public const string Export = "pricing:export";

                public static readonly string[] AllPermissions = { Read, Create, Access, Update, Delete, Export };
            }
        }

        public static class Settings
        {
            public static class General
            {
                public static readonly SettingDescriptor ExportImportPageSize = new SettingDescriptor
                {
                    Name = "Pricing.ExportImport.PageSize",
                    GroupName = "Pricing|General",
                    ValueType = SettingValueType.Integer,
                    DefaultValue = 50
                };

                public static SettingDescriptor IndexationDatePricingCalendar = new SettingDescriptor
                {
                    Name = "VirtoCommerce.Search.IndexingJobs.IndexationDate.Pricing.Calendar",
                    GroupName = "Pricing|Search",
                    ValueType = SettingValueType.DateTime,
                    DefaultValue = default(DateTime)
                };

                public static SettingDescriptor PricingIndexing = new SettingDescriptor
                {
                    Name = "Pricing.Indexing.Enable",
                    GroupName = "Pricing|Search",
                    ValueType = SettingValueType.Boolean,
                    DefaultValue = true
                };

                public static IEnumerable<SettingDescriptor> AllSettings
                {
                    get
                    {
                        yield return ExportImportPageSize;
                        yield return IndexationDatePricingCalendar;
                        yield return PricingIndexing;
                    }
                }
            }

            public static IEnumerable<SettingDescriptor> AllSettings => General.AllSettings;
        }
    }
}
