using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.Platform.Core;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Modularity;

namespace VirtoCommerce.Platform.Web.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UsePlatformSettings(this IApplicationBuilder appBuilder)
        {
            var moduleCatalog = appBuilder.ApplicationServices.GetRequiredService<ILocalModuleCatalog>();

            var platforModuleManifest = new ManifestModuleInfo(new ModuleManifest
            {
                Id = "VirtoCommerce.Platform",
                Version = PlatformVersion.CurrentVersion.ToString(),
                PlatformVersion = PlatformVersion.CurrentVersion.ToString()
            });

            platforModuleManifest.Settings.Add(new ModuleSettingsGroup
            {
                Name = "Platform|Security",
                Settings = PlatformConstants.Settings.Security.AllSettings.ToArray()
            });
            platforModuleManifest.Settings.Add(new ModuleSettingsGroup
            {
                Name = "Platform|Cache",
                Settings = PlatformConstants.Settings.Cache.AllSettings.ToArray()
            });
            platforModuleManifest.Settings.Add(new ModuleSettingsGroup
            {
                Name = "Platform|Setup",
                Settings = PlatformConstants.Settings.Setup.AllSettings.ToArray()
            });
            platforModuleManifest.Settings.Add(new ModuleSettingsGroup
            {
                Name = "Platform|User Profile",
                Settings = PlatformConstants.Settings.UserProfile.AllSettings.ToArray()
            });
            platforModuleManifest.Settings.Add(new ModuleSettingsGroup
            {
                Name = "Platform|User Interface",
                Settings = PlatformConstants.Settings.UserInterface.AllSettings.ToArray()
            });

            moduleCatalog.AddModule(platforModuleManifest);

            return appBuilder;
        }
    }

}
