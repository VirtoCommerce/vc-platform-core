using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Modules.External;

namespace VirtoCommerce.Platform.Modules
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddModules(this IServiceCollection services, IMvcBuilder mvcBuilder, Action<LocalStorageModuleCatalogOptions> setupAction = null)
        {
            services.AddSingleton(services);

            services.AddSingleton<IModuleInitializer, ModuleInitializer>();
            services.AddSingleton<IAssemblyResolver, LoadContextAssemblyResolver>();
            services.AddSingleton<IModuleManager, ModuleManager>();
            services.AddSingleton<ILocalModuleCatalog, LocalStorageModuleCatalog>();
            services.AddSingleton<IModuleCatalog>( provider => provider.GetService<ILocalModuleCatalog>());
            services.AddSingleton<IAssemblyResolver, LoadContextAssemblyResolver>();

            if (setupAction != null)
            {
                services.Configure(setupAction);
            }
            var providerSnapshot = services.BuildServiceProvider();

            var manager = providerSnapshot.GetRequiredService<IModuleManager>();
            var moduleCatalog = providerSnapshot.GetRequiredService<ILocalModuleCatalog>();

            manager.Run();
            // Ensure all modules are loaded
            foreach (var module in moduleCatalog.Modules.OfType<ManifestModuleInfo>().Where(x => x.State == ModuleState.NotStarted))
            {
                manager.LoadModule(module.ModuleName);
                // Register API controller from modules
                mvcBuilder.AddApplicationPart(module.Assembly);
            }

            services.AddSingleton(moduleCatalog);
            return services;
        }
  
        public static IServiceCollection AddExternalModules(this IServiceCollection services, Action<ExternalModuleCatalogOptions> setupAction = null)
        {
            services.AddSingleton<IExternalModulesClient, ExternalModulesClient>();
            services.AddSingleton<IExternalModuleCatalog, ExternalModuleCatalog>();
            services.AddSingleton<IModuleInstaller, ModuleInstaller>();

            if (setupAction != null)
            {
                services.Configure(setupAction);
            }

            return services;
        }
    }
}
