using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.ExportModule.Core.Model;
using VirtoCommerce.ExportModule.Core.Services;
using VirtoCommerce.ExportModule.Data.Services;
using VirtoCommerce.Platform.Core.Modularity;

namespace VirtoCommerce.ExportModule.Web
{
    public class Module : IModule
    {
        public ManifestModuleInfo ModuleInfo { get; set; }

        public void Initialize(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<KnownExportTypesService>();
            serviceCollection.AddSingleton<IKnownExportTypesRegistrar>(serviceProvider => serviceProvider.GetRequiredService<KnownExportTypesService>());
            serviceCollection.AddSingleton<IKnownExportTypesResolver>(serviceProvider => serviceProvider.GetRequiredService<KnownExportTypesService>());

            serviceCollection.AddTransient<Func<IExportProviderConfiguration, Stream, IExportProvider>>(serviceProvider => (config, stream) => new JsonExportProvider(stream, config));
            serviceCollection.AddTransient<Func<IExportProviderConfiguration, Stream, IExportProvider>>(serviceProvider => (config, stream) => new CsvExportProvider(stream, config));
            serviceCollection.AddTransient<IExportProviderFactory, ExportProviderFactory>();

            serviceCollection.AddScoped<IDataExporter, DataExporter>();
        }

        public void PostInitialize(IApplicationBuilder appBuilder)
        {
        }

        public void Uninstall()
        {
        }
    }
}
