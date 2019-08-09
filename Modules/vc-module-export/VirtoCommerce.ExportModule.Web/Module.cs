using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.ExportModule.Core;
using VirtoCommerce.ExportModule.Core.Model;
using VirtoCommerce.ExportModule.Core.Services;
using VirtoCommerce.ExportModule.Data.Services;
using VirtoCommerce.ExportModule.Web.JsonConverters;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Core.Security;

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

            serviceCollection.AddTransient<Func<IExportProviderConfiguration, ExportedTypeColumnInfo[], IExportProvider>>(serviceProvider => (config, includedColumns) => new JsonExportProvider(config, includedColumns));
            serviceCollection.AddTransient<Func<IExportProviderConfiguration, ExportedTypeColumnInfo[], IExportProvider>>(serviceProvider => (config, includedColumns) => new CsvExportProvider(config, includedColumns));
            serviceCollection.AddTransient<IExportProviderFactory, ExportProviderFactory>();

            serviceCollection.AddTransient<IDataExporter, DataExporter>();

            serviceCollection.Configure<MvcJsonOptions>(configure =>
            {
                configure.SerializerSettings.Converters.Add(new PolymorphicExportDataQueryJsonConverter());
            });

        }

        public void PostInitialize(IApplicationBuilder appBuilder)
        {
            //Register module permissions
            var permissionsProvider = appBuilder.ApplicationServices.GetRequiredService<IPermissionsRegistrar>();
            permissionsProvider.RegisterPermissions(ModuleConstants.Security.Permissions.AllPermissions.Select(x => new Permission() { GroupName = "Export", Name = x }).ToArray());
        }

        public void Uninstall()
        {
        }
    }
}
