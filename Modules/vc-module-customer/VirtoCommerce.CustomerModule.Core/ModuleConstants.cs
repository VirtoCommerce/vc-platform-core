using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Settings;

namespace VirtoCommerce.CustomerModule.Core
{
    public static class ModuleConstants
    {
        public static class Security
        {
            public static class Permissions
            {
                public const string Read = "customer:read";
                public const string Create = "customer:create";
                public const string Access = "customer:access";
                public const string Update = "customer:update";
                public const string Delete = "customer:delete";

                public static string[] AllPermissions = new[] { Read, Create, Access, Update, Delete };
            }
        }

        public static class Settings
        {
            public static class General
            {
                public static SettingDescriptor MemberGroups = new SettingDescriptor
                {
                    Name = "Customer.MemberGroups",
                    GroupName = "Customer|General",
                    ValueType = SettingValueType.ShortText,
                    IsDictionary = true,
                    DefaultValue = "New",
                    AllowedValues = new[] { "VIP", "Wholesaler" }
                };

                public static IEnumerable<SettingDescriptor> AllSettings
                {
                    get
                    {
                        yield return MemberGroups;
                    }
                }
            }

            public static IEnumerable<SettingDescriptor> AllSettings
            {
                get
                {
                    return General.AllSettings;
                }
            }
        }

    }
}
