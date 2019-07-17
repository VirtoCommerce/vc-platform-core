using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.ExportImport;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.Platform.Data.Extensions;
using VirtoCommerce.ShippingModule.Core;
using VirtoCommerce.ShippingModule.Core.Services;
using VirtoCommerce.ShippingModule.Data;
using VirtoCommerce.ShippingModule.Data.ExportImport;
using VirtoCommerce.ShippingModule.Data.Repositories;
using VirtoCommerce.ShippingModule.Data.Services;
using VirtoCommerce.ShippingModule.Web.JsonConverters;

namespace VirtoCommerce.ShippingModule.Web
{
    public class Module : IModule, IExportSupport, IImportSupport
    {
        public ManifestModuleInfo ModuleInfo { get; set; }

        private IApplicationBuilder _appBuilder;
        public void Initialize(IServiceCollection serviceCollection)
        {
            var snapshot = serviceCollection.BuildServiceProvider();
            var configuration = snapshot.GetService<IConfiguration>();
            var connectionString = configuration.GetConnectionString("VirtoCommerce.Shipping") ?? configuration.GetConnectionString("VirtoCommerce");
            serviceCollection.AddDbContext<ShippingDbContext>(options => options.UseSqlServer(connectionString));
            serviceCollection.AddTransient<IShippingRepository, ShippingRepository>();
            serviceCollection.AddTransient<Func<IShippingRepository>>(provider => () => provider.CreateScope().ServiceProvider.GetService<IShippingRepository>());

            serviceCollection.AddTransient<IShippingMethodsService, ShippingMethodsService>();
            serviceCollection.AddTransient<IShippingMethodsRegistrar, ShippingMethodsService>();
            serviceCollection.AddTransient<IShippingMethodsSearchService, ShippingMethodsSearchService>();
            serviceCollection.AddTransient<ShippingExportImport>();
        }

        public void PostInitialize(IApplicationBuilder applicationBuilder)
        {
            _appBuilder = applicationBuilder;

            var settingsRegistrar = applicationBuilder.ApplicationServices.GetRequiredService<ISettingsRegistrar>();
            settingsRegistrar.RegisterSettings(ModuleConstants.Settings.AllSettings, ModuleInfo.Id);
            settingsRegistrar.RegisterSettingsForType(ModuleConstants.Settings.FixedRateShippingMethod.AllSettings, typeof(FixedRateShippingMethod).Name);

            var shippingMethodsRegistrar = applicationBuilder.ApplicationServices.GetRequiredService<IShippingMethodsRegistrar>();
            shippingMethodsRegistrar.RegisterShippingMethod<FixedRateShippingMethod>();

            var mvcJsonOptions = applicationBuilder.ApplicationServices.GetService<IOptions<MvcJsonOptions>>();
            mvcJsonOptions.Value.SerializerSettings.Converters.Add(new PolymorphicJsonConverter());

            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var dbContext = serviceScope.ServiceProvider.GetRequiredService<ShippingDbContext>();
                dbContext.Database.MigrateIfNotApplied(MigrationName.GetUpdateV2MigrationName(ModuleInfo.Id));
                dbContext.Database.EnsureCreated();
                dbContext.Database.Migrate();
            }
        }

        public void Uninstall()
        {
        }

        public async Task ExportAsync(Stream outStream, ExportImportOptions options, Action<ExportImportProgressInfo> progressCallback,
          ICancellationToken cancellationToken)
        {
            await _appBuilder.ApplicationServices.GetRequiredService<ShippingExportImport>().DoExportAsync(outStream, progressCallback, cancellationToken);
        }

        public async Task ImportAsync(Stream inputStream, ExportImportOptions options, Action<ExportImportProgressInfo> progressCallback,
            ICancellationToken cancellationToken)
        {
            await _appBuilder.ApplicationServices.GetRequiredService<ShippingExportImport>().DoImportAsync(inputStream, progressCallback, cancellationToken);
        }
    }
}
