using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.Platform.Web.Infrastructure;
using VirtoCommerce.Platform.Web.Middelware;

namespace VirtoCommerce.Platform.Web.Extensions
{
    public static class ApplicationBuilderExtensions
    {
    
        public static IApplicationBuilder UseVirtualFolders(this IApplicationBuilder appBuilder, Action<VirtualFolderOptions> configureVirtualFolders)
        {
            if (configureVirtualFolders != null)
            {
                var virtualFolderOptions = appBuilder.ApplicationServices.GetRequiredService<IOptions<VirtualFolderOptions>>().Value;
                configureVirtualFolders(virtualFolderOptions);

                var rewriteOptions = new RewriteOptions().Add(new VirtualFoldersUrlRewriteRule(virtualFolderOptions));
                appBuilder.UseRewriter(rewriteOptions);
            }
           
            return appBuilder;
        }

        public static IApplicationBuilder UsePlatformSettings(this IApplicationBuilder appBuilder)
        {
            var moduleCatalog = appBuilder.ApplicationServices.GetRequiredService<ILocalModuleCatalog>();

            var platformModuleManifest = new ModuleManifest
            {
                Id = "VirtoCommerce.Platform",
                Version = PlatformVersion.CurrentVersion.ToString(),
                PlatformVersion = PlatformVersion.CurrentVersion.ToString(),
                Settings = new[]
               {                 
                    new ModuleSettingsGroup
                    {
                        Name = "Platform|User Profile",
                        Settings = new[]
                        {
                            new ModuleSetting
                            {
                                Name = "VirtoCommerce.Platform.UI.MainMenu.State",
                                ValueType = ModuleSetting.TypeJson,
                                Title = "Persisted state of main menu"
                            },
                            new ModuleSetting
                            {
                                Name = "VirtoCommerce.Platform.UI.Language",
                                ValueType = ModuleSetting.TypeString,
                                Title = "Language",
                                Description = "Default language (two letter code from ISO 639-1, case-insensitive). Example: en, de",
                                DefaultValue = "en"
                            },
                            new ModuleSetting
                            {
                                Name = "VirtoCommerce.Platform.UI.RegionalFormat",
                                ValueType = ModuleSetting.TypeString,
                                Title = "Regional format",
                                Description = "Default regional format (CLDR locale code, with dash or underscore as delemiter, case-insensitive). Example: en, en_US, sr_Cyrl, sr_Cyrl_RS",
                                DefaultValue = "en"
                            },
                            new ModuleSetting
                            {
                                Name = "VirtoCommerce.Platform.UI.TimeZone",
                                ValueType = ModuleSetting.TypeString,
                                Title = "Time zone",
                                Description = "Default time zone (IANA time zone name [tz database], exactly as in database, case-sensitive). Examples: America/New_York, Europe/Moscow"
                            },
                            new ModuleSetting
                            {
                                Name = "VirtoCommerce.Platform.UI.UseTimeAgo",
                                ValueType = ModuleSetting.TypeBoolean,
                                Title = "Use time ago format when is possible",
                                Description = "When set to true (by default), system will display date in format like 'a few seconds ago' when possible",
                                DefaultValue = true.ToString()
                            },
                            new ModuleSetting
                            {
                                Name = "VirtoCommerce.Platform.UI.FullDateThreshold",
                                ValueType = ModuleSetting.TypeInteger,
                                Title = "Full date threshold",
                                Description = "Number of units after time ago format will be switched to full date format"
                            },
                            new ModuleSetting
                            {
                                Name = "VirtoCommerce.Platform.UI.FullDateThresholdUnit",
                                ValueType = ModuleSetting.TypeString,
                                Title = "Full date threshold unit",
                                Description = "Unit of full date threshold",
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
                            }
                        }
                    },
                    new ModuleSettingsGroup
                    {
                        Name = "Platform|User Interface",
                        Settings = new[]
                        {
                            new ModuleSetting
                            {
                                Name = "VirtoCommerce.Platform.UI.Customization",
                                ValueType = ModuleSetting.TypeJson,
                                Title = "Customization",
                                Description = "JSON contains personalization settings of manager UI",
                                DefaultValue = "{\n" +
                                               "  \"title\": \"Virto Commerce\",\n" +
                                               "  \"logo\": \"Content/themes/main/images/logo.png\",\n" +
                                               "  \"contrast_logo\": \"Content/themes/main/images/contrast-logo.png\"\n" +
                                               "}"
                            }
                        }
                    }
                }
            };

            moduleCatalog.AddModule(new ManifestModuleInfo(platformModuleManifest));

            return appBuilder;
        }
    }

}
