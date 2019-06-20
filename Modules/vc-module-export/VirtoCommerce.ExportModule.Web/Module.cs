using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using VirtoCommerce.ExportModule.Core.Model;
using VirtoCommerce.ExportModule.Core.Services;
using VirtoCommerce.ExportModule.Data.Services;
using VirtoCommerce.ExportModule.Web.Converters;
using VirtoCommerce.Platform.Core.Modularity;

namespace VirtoCommerce.ExportModule.Web
{
    public class Module : IModule
    {
        public ManifestModuleInfo ModuleInfo { get; set; }

        public void Initialize(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IKnownExportTypesRegistrar, KnownExportTypesService>();
            serviceCollection.AddSingleton<IKnownExportTypesResolver, KnownExportTypesService>();

            serviceCollection.AddTransient<Func<IExportProviderConfiguration, Stream, IExportProvider>>(serviceProvider => (config, stream) => new JsonExportProvider(stream, config));
            serviceCollection.AddTransient<Func<IExportProviderConfiguration, Stream, IExportProvider>>(serviceProvider => (config, stream) => new CsvExportProvider(stream, config));
            serviceCollection.AddTransient<IExportProviderFactory, ExportProviderFactory>();

            serviceCollection.AddScoped<IDataExporter, DataExporter>();
        }

        public void PostInitialize(IApplicationBuilder appBuilder)
        {
            var mvcJsonOptions = appBuilder.ApplicationServices.GetService<IOptions<MvcJsonOptions>>();
            mvcJsonOptions.Value.SerializerSettings.Converters.Add(new PolymorphicJsonConverter());
        }

        public void Uninstall()
        {
        }
    }
}
