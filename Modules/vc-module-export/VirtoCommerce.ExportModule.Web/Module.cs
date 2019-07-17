using System;
using System.Linq;
using Hangfire.Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
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

            serviceCollection.AddTransient<Func<IExportProviderConfiguration, IExportProvider>>(serviceProvider => config => new JsonExportProvider(config));
            serviceCollection.AddTransient<Func<IExportProviderConfiguration, IExportProvider>>(serviceProvider => config => new CsvExportProvider(config));
            serviceCollection.AddTransient<IExportProviderFactory, ExportProviderFactory>();

            serviceCollection.AddTransient<IDataExporter, DataExporter>();
        }

        public void PostInitialize(IApplicationBuilder appBuilder)
        {
            var mvcJsonOptions = appBuilder.ApplicationServices.GetRequiredService<IOptions<MvcJsonOptions>>();

            mvcJsonOptions.Value.SerializerSettings.Converters.Add(new PolymorphicExportDataQueryJsonConverter());

            // This line refreshes Hangfire JsonConverter with the current JsonSerializerSettings - PolymorphicExportDataQueryJsonConverter needs to be included
            JobHelper.SetSerializerSettings(mvcJsonOptions.Value.SerializerSettings);

            //Register module permissions
            var permissionsProvider = appBuilder.ApplicationServices.GetRequiredService<IPermissionsRegistrar>();
            permissionsProvider.RegisterPermissions(ModuleConstants.Security.Permissions.AllPermissions.Select(x => new Permission() { GroupName = "Export", Name = x }).ToArray());
        }

        public void Uninstall()
        {
        }
    }
}
