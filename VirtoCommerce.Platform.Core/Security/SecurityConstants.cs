using System;
using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Modularity;

namespace VirtoCommerce.Platform.Core.Security
{
    public static class SecurityConstants
    {
        public static class Settings
        {
            public static ModuleSetting SecurityAccountTypes = new ModuleSetting
            {
                Name = "VirtoCommerce.Platform.Security.AccountTypes",
                ValueType = ModuleSetting.TypeString,
                IsArray = true,
                ArrayValues = Enum.GetNames(typeof(UserType)),
                DefaultValue = UserType.Manager.ToString()
            };

            public static IEnumerable<ModuleSetting> AllSettings
            {
                get
                {
                    yield return SecurityAccountTypes;
                }
            }
        }

        public static class Claims
        {
            public const string PermissionClaimType = "permission";
            public const string UserNameClaimType = "username";
        }

        public static class Roles
        {
            public const string Customer = "Customer";
            public const string Manager = "Manager";
            public const string Administrator = "Administrator";
        }

        public static class Permissions
        {
            public const string AssetAccess = "platform:asset:access",
              AssetDelete = "platform:asset:delete",
              AssetUpdate = "platform:asset:update",
              AssetCreate = "platform:asset:create",
              AssetRead = "platform:asset:read";

            public const string ModuleQuery = "platform:module:read",
                ModuleAccess = "platform:module:access",
                ModuleManage = "platform:module:manage";
            public const string SettingQuery = "platform:setting:read",
                SettingAccess = "platform:setting:access",
                SettingUpdate = "platform:setting:update";
            public const string DynamicPropertiesQuery = "platform:dynamic_properties:read",
                DynamicPropertiesCreate = "platform:dynamic_properties:create",
                DynamicPropertiesAccess = "platform:dynamic_properties:access",
                DynamicPropertiesUpdate = "platform:dynamic_properties:update",
                DynamicPropertiesDelete = "platform:dynamic_properties:delete";
            public const string SecurityQuery = "platform:security:read",
                SecurityCreate = "platform:security:create",
                SecurityAccess = "platform:security:access",
                SecurityUpdate = "platform:security:update",
                SecurityDelete = "platform:security:delete";
            public const string SecurityCallApi = "security:call_api";
            public const string BackgroundJobsManage = "background_jobs:manage";
            public const string PlatformExportImportAccess = "platform:exportImport:access",
                PlatformImport = "platform:import",
                PlatformExport = "platform:export";

            public static string[] AllPermissions = new[] { AssetAccess, AssetDelete, AssetUpdate, AssetCreate, AssetRead, ModuleQuery, ModuleAccess, ModuleManage,
                                              SettingQuery, SettingAccess, SettingUpdate, DynamicPropertiesQuery, DynamicPropertiesCreate, DynamicPropertiesAccess, DynamicPropertiesUpdate, DynamicPropertiesDelete,
                                              SecurityQuery, SecurityCreate, SecurityAccess,  SecurityUpdate,  SecurityDelete, SecurityCallApi, BackgroundJobsManage, PlatformExportImportAccess, PlatformImport, PlatformExport};
        }
    }
}
