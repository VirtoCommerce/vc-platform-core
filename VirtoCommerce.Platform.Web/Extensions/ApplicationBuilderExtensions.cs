using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Core.Security;

namespace VirtoCommerce.Platform.Web.Extensions
{
    public static class ApplicationBuilderExtensions
    {
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
                        Name = "Platform|Security",
                        Settings = SecurityConstants.Settings.AllSettings.ToArray()
                     },
                    new ModuleSettingsGroup
                    {
                        Name = "Platform|Setup",
                        Settings = PlatformConstants.Settings.Setup.AllSettings.ToArray()
                    },
                    new ModuleSettingsGroup
                    {
                        Name = "Platform|User Profile",
                        Settings = PlatformConstants.Settings.UserProfile.AllSettings.ToArray()
                    },
                    new ModuleSettingsGroup
                    {
                        Name = "Platform|User Interface",
                        Settings = PlatformConstants.Settings.UserInterface.AllSettings.ToArray()
                    }
                }
            };

            moduleCatalog.AddModule(new ManifestModuleInfo(platformModuleManifest));

            return appBuilder;
        }
    }

}
