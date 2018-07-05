using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Modularity;

namespace VirtoCommerce.NotificationsModule.Core
{
    public static class ModuleConstants
    {
        public static class Security
        {
            public static class Permissions
            {
                public const string Read = "notifications:read";
                public const string Create = "notifications:create";
                public const string Access = "notifications:access";
                public const string Update = "notifications:update";
                public const string Delete = "notifications:delete";
                public const string Export = "notifications:export";
                public const string Import = "notifications:import";
                public const string ReadTemplates = "notifications:templates:read";
                public const string CreateTemplate = "notifications:template:create";

                public static string[] AllPermissions = new[] { Read, Create, Access, Update, Delete, Export, Import, ReadTemplates, CreateTemplate };
            }
        }

        public static class Settings
        {
            public static class General
            {
                public static ModuleSetting Languages = new ModuleSetting
                {
                    Name = "VirtoCommerce.Notifications.General.Languages",
                    ValueType = ModuleSetting.TypeString,
                    IsArray = true,
                    DefaultValue = "en-US",
                    ArrayValues = new[] { "en-US", "de-DE" }
                };

                public static IEnumerable<ModuleSetting> AllSettings
                {
                    get
                    {
                        yield return Languages;
                    }
                }
            }

            
        }

    }
}
