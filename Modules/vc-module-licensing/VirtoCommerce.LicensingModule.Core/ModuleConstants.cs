using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Settings;

namespace VirtoCommerce.LicensingModule.Core
{
    public class ModuleConstants
    {
        public static class Security
        {
            public static class Permissions
            {
                public const string Read = "licensing:read";
                public const string Create = "licensing:create";
                public const string Access = "licensing:access";
                public const string Update = "licensing:update";
                public const string Delete = "licensing:delete";
                public const string Issue = "licensing:issue";

                public static string[] AllPermissions = { Read, Create, Access, Update, Delete, Issue };
            }
        }

        public static class Settings
        {
            public static class General
            {
                public static SettingDescriptor LicenseType = new SettingDescriptor
                {
                    Name = "Licensing.LicenseType",
                    GroupName = "Licensing|General",
                    ValueType = SettingValueType.ShortText,
                    IsDictionary = true,
                    DefaultValue = "Commerce",
                    AllowedValues = new[] { "Commerce", "Community deployment" }
                };

                public static IEnumerable<SettingDescriptor> AllSettings
                {
                    get
                    {
                        yield return LicenseType;
                    }
                }
            }
            public static IEnumerable<SettingDescriptor> AllSettings => General.AllSettings;
        }
    }
}
