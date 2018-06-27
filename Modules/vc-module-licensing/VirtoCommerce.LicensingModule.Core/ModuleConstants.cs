using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Modularity;

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

                public static string[] AllPermissions = new[] { Read, Create, Access, Update, Delete, Issue };
            }
        }

        public static class Settings
        {
            public static class General
            {
                public static ModuleSetting LicenseType = new ModuleSetting
                {
                    Name = "Licensing.LicenseType",
                    ValueType = ModuleSetting.TypeString,
                    IsArray = true,
                    DefaultValue = "Commerce",
                    ArrayValues = new [] { "Commerce", "Community deployment" }
                };

                public static ModuleSetting SignaturePrivateKey = new ModuleSetting
                {
                    Name = "Licensing.SignaturePrivateKey",
                    ValueType = ModuleSetting.TypeText
                };

                public static IEnumerable<ModuleSetting> AllSettings
                {
                    get
                    {
                        yield return LicenseType;
                        yield return SignaturePrivateKey;
                    }
                }
            }
        }
    }
}
