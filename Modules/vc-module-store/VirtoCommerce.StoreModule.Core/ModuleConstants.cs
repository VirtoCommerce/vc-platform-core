using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Modularity;

namespace VirtoCommerce.StoreModule.Core
{
    public static class ModuleConstants
    {
        public static class Security
        {
            public static class Permissions
            {
                public const string Read = "store:read",
                    Create = "store:create",
                    Access = "store:access",
                    Update = "store:update",
                    Delete = "store:delete",
                    LoginOnBehalf = "store:loginOnBehalf";

                public static string[] AllPermissions = new[] { Access, Create, Read, Update, Delete, LoginOnBehalf };
            }
        }

        public static class Settings
        {
            public static class General
            {
                public static ModuleSetting States = new ModuleSetting
                {
                    Name = "Stores.States",
                    ValueType = ModuleSetting.TypeString,
                    IsArray = true,
                    DefaultValue = "Open",
                    ArrayValues = new [] { "Open", "Closed", "RestrictedAccess" }
                };

                public static ModuleSetting TaxCalculationEnabled = new ModuleSetting
                {
                    Name = "Stores.TaxCalculationEnabled",
                    ValueType = ModuleSetting.TypeBoolean,
                    DefaultValue = true.ToString(),
                };

                public static IEnumerable<ModuleSetting> AllSettings
                {
                    get
                    {
                        yield return States;
                        yield return TaxCalculationEnabled;
                    }
                }
            }

            public static class SEO
            {
                public static ModuleSetting SeoLinksType = new ModuleSetting
                {
                    Name = "Stores.SeoLinksType",
                    ValueType = ModuleSetting.TypeString,
                    IsArray = true,
                    DefaultValue = "Collapsed",
                    AllowedValues = new[] { "None", "Short", "Collapsed", "Long" }
                };

                public static IEnumerable<ModuleSetting> AllSettings
                {
                    get
                    {
                        yield return SeoLinksType;
                    }
                }
            }
        }
    }
}
