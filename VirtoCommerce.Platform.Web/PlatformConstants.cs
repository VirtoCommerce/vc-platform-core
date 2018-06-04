using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Modularity;

namespace VirtoCommerce.Platform.Web
{
    public static class PlatformConstants
    {
        public static class Settings
        {
            public static class Setup
            {
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
                    }
                }
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
