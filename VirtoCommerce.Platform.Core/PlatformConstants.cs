using System;
using System.Collections.Generic;
using VirtoCommerce.Platform.Core.ExportImport;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Core.Security;

namespace VirtoCommerce.Platform.Core
{
    public static class PlatformConstants
    {
        public static class Security
        {
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

        public static class Settings
        {
            public static class Cache
            {
                public static ModuleSetting CacheEnabled = new ModuleSetting
                {
                    Name = "VirtoCommerce.Platform.Cache.Enabled",
                    ValueType = ModuleSetting.TypeBoolean,
                    DefaultValue = true.ToString(),
                    RestartRequired = true

                };
                public static ModuleSetting AbsoluteExpiration = new ModuleSetting
                {
                    Name = "VirtoCommerce.Platform.Cache.AbsoulteExpiration",
                    ValueType = ModuleSetting.TypeString,
                    DefaultValue = TimeSpan.FromDays(1).ToString(),
                    RestartRequired = true
                };

                public static IEnumerable<ModuleSetting> AllSettings
                {
                    get
                    {
                        yield return CacheEnabled;
                        yield return AbsoluteExpiration;
                    }
                }
            }

            public static class Security
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

            public static class Setup
            {
                public static ModuleSetting SampleDataState = new ModuleSetting
                {
                    Name = "VirtoCommerce.SampleDataState",
                    ValueType = ModuleSetting.TypeString,
                    DefaultValue = ExportImport.SampleDataState.Undefined.ToString()
                };
                public static ModuleSetting ModulesAutoInstallState = new ModuleSetting
                {
                    Name = "VirtoCommerce.ModulesAutoInstallState",
                    ValueType = ModuleSetting.TypeString,
                    DefaultValue = AutoInstallState.Undefined.ToString()
                };

                public static ModuleSetting ModulesAutoInstalled = new ModuleSetting
                {
                    Name = "VirtoCommerce.ModulesAutoInstalled",
                    ValueType = ModuleSetting.TypeBoolean,
                    DefaultValue = false.ToString()
                };

                public static IEnumerable<ModuleSetting> AllSettings
                {
                    get
                    {
                        yield return ModulesAutoInstalled;
                        yield return ModulesAutoInstallState;
                        yield return SampleDataState;
                    }
                }
            }

            public static class Modularity
            {
                public static ModuleSetting ModulesAutoInstalled = new ModuleSetting
                {
                    Name = "VirtoCommerce.ModulesAutoInstalled",
                    ValueType = ModuleSetting.TypeBoolean,
                    DefaultValue = false.ToString()
                };
                public static ModuleSetting ModulesAutoInstallState = new ModuleSetting
                {
                    Name = "VirtoCommerce.ModulesAutoInstallState",
                    ValueType = ModuleSetting.TypeSecureString,
                    DefaultValue = AutoInstallState.Undefined.ToString()
                };

            }

            public static class UserProfile
            {
                public static ModuleSetting MainMenuState = new ModuleSetting
                {
                    Name = "VirtoCommerce.Platform.UI.MainMenu.State",
                    ValueType = ModuleSetting.TypeJson,
                };
                public static ModuleSetting Language = new ModuleSetting
                {
                    Name = "VirtoCommerce.Platform.UI.Language",
                    ValueType = ModuleSetting.TypeString,
                    DefaultValue = "en"
                };
                public static ModuleSetting RegionalFormat = new ModuleSetting
                {
                    Name = "VirtoCommerce.Platform.UI.RegionalFormat",
                    ValueType = ModuleSetting.TypeString,
                    DefaultValue = "en"
                };
                public static ModuleSetting TimeZone = new ModuleSetting
                {
                    Name = "VirtoCommerce.Platform.UI.TimeZone",
                    ValueType = ModuleSetting.TypeString
                };
                public static ModuleSetting UseTimeAgo = new ModuleSetting
                {
                    Name = "VirtoCommerce.Platform.UI.UseTimeAgo",
                    ValueType = ModuleSetting.TypeBoolean,
                    DefaultValue = true.ToString()
                };
                public static ModuleSetting FullDateThreshold = new ModuleSetting
                {
                    Name = "VirtoCommerce.Platform.UI.FullDateThreshold",
                    ValueType = ModuleSetting.TypeInteger
                };
                public static ModuleSetting FullDateThresholdUnit = new ModuleSetting
                {
                    Name = "VirtoCommerce.Platform.UI.FullDateThresholdUnit",
                    ValueType = ModuleSetting.TypeString,
                    DefaultValue = "Never",
                    AllowedValues = new[]
                                {
                                    "Never",
                                    "Seconds",
                                    "Minutes",
                                    "Hours",
                                    "Days",
                                    "Weeks",
                                    "Months",
                                    "Quarters",
                                    "Years"
                                }
                };

                public static ModuleSetting ShowMeridian = new ModuleSetting
                {
                    Name = "VirtoCommerce.Platform.UI.ShowMeridian",
                    ValueType = ModuleSetting.TypeBoolean,
                    DefaultValue = true.ToString()
                };

                public static IEnumerable<ModuleSetting> AllSettings
                {
                    get
                    {
                        yield return MainMenuState;
                        yield return Language;
                        yield return RegionalFormat;
                        yield return TimeZone;
                        yield return UseTimeAgo;
                        yield return FullDateThreshold;
                        yield return FullDateThresholdUnit;
                        yield return ShowMeridian;
                    }
                }
            }
            public static class UserInterface
            {
                public static ModuleSetting Customization = new ModuleSetting
                {
                    Name = "VirtoCommerce.Platform.UI.Customization",
                    ValueType = ModuleSetting.TypeJson,
                    DefaultValue = "{\n" +
                                               "  \"title\": \"Virto Commerce\",\n" +
                                               "  \"logo\": \"/images/logo.png\",\n" +
                                               "  \"contrast_logo\": \"/images/contrast-logo.png\"\n" +
                                               "}"
                };
                public static IEnumerable<ModuleSetting> AllSettings
                {
                    get
                    {
                        yield return Customization;
                    }
                }
            }
        }
    }
}
