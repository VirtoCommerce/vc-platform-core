using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Modularity;

namespace VirtoCommerce.Platform.Modules.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseModules(this IApplicationBuilder appBuilder)
        {
            using (var serviceScope = appBuilder.ApplicationServices.CreateScope())
            {
                var moduleManager = serviceScope.ServiceProvider.GetRequiredService<IModuleManager>();
                var modules = GetInstalledModules(serviceScope.ServiceProvider).ToArray();

                foreach (var moduleInfo in modules)
                {
                    if (moduleInfo.PostInitializeState == ModuleState.NotStarted)
                    {
                        moduleInfo.PostInitializeState = ModuleState.ReadyForInitialization;
                    }
                }

                var keepLoading = true;
                while (keepLoading)
                {
                    keepLoading = false;
                    var availableModules = modules.Where(m => m.PostInitializeState == ModuleState.ReadyForInitialization);

                    foreach (var moduleInfo in availableModules)
                    {
                        if ((moduleInfo.PostInitializeState != ModuleState.Initialized) && (AreDependenciesLoaded(moduleInfo, modules)))
                        {
                            moduleInfo.PostInitializeState = ModuleState.Initializing;
                            moduleManager.PostInitializeModule(moduleInfo, appBuilder);
                            moduleInfo.PostInitializeState = ModuleState.Initialized;
                            keepLoading = true;
                            break;
                        }
                    }
                }
            }
            return appBuilder;
        }

        private static IEnumerable<ManifestModuleInfo> GetInstalledModules(IServiceProvider serviceProvider)
        {
            var moduleCatalog = serviceProvider.GetRequiredService<ILocalModuleCatalog>();
            var allModules = moduleCatalog.Modules.OfType<ManifestModuleInfo>().ToArray();
            return moduleCatalog.CompleteListWithDependencies(allModules)
                .OfType<ManifestModuleInfo>()
                .Where(x => x.State == ModuleState.Initialized && !x.Errors.Any())
                .OrderBy(m => m.Id)
                .ToArray();
        }

        private static bool AreDependenciesLoaded(ManifestModuleInfo manifestModuleInfo, ManifestModuleInfo[] modules)
        {
            var result = false;

            if (manifestModuleInfo.Dependencies.IsNullCollection())
            {
                result = true;
            }
            else if (manifestModuleInfo.Dependencies.All(d =>
                modules.SingleOrDefault(m => m.Id.EqualsInvariant(d.Id))?.PostInitializeState == ModuleState.Initialized))
            {
                result = true;
            }

            return result;
        }
    }
}
