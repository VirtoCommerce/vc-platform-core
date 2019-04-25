using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using VirtoCommerce.InventoryModule.Core;
using VirtoCommerce.InventoryModule.Core.Services;
using VirtoCommerce.InventoryModule.Data.ExportImport;
using VirtoCommerce.InventoryModule.Data.Repositories;
using VirtoCommerce.InventoryModule.Data.Search.Indexing;
using VirtoCommerce.InventoryModule.Data.Services;
using VirtoCommerce.InventoryModule.Web.JsonConverters;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.ExportImport;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.Platform.Data.Extensions;
using VirtoCommerce.SearchModule.Core.Model;

namespace VirtoCommerce.InventoryModule.Web
{
    public class Module : IModule, IExportSupport, IImportSupport
    {
        public ManifestModuleInfo ModuleInfo { get; set; }

        private IApplicationBuilder _appBuilder;

        public void Initialize(IServiceCollection serviceCollection)
        {
            var configuration = serviceCollection.BuildServiceProvider().GetRequiredService<IConfiguration>();
            serviceCollection.AddTransient<IInventoryRepository, InventoryRepositoryImpl>();
            var connectionString = configuration.GetConnectionString("VirtoCommerce.Inventory") ?? configuration.GetConnectionString("VirtoCommerce");
            serviceCollection.AddDbContext<InventoryDbContext>(options => options.UseSqlServer(connectionString));
            serviceCollection.AddSingleton<Func<IInventoryRepository>>(provider => () => provider.CreateScope().ServiceProvider.GetRequiredService<IInventoryRepository>());
            serviceCollection.AddSingleton<IInventoryService, InventoryServiceImpl>();
            serviceCollection.AddSingleton<IInventorySearchService, InventorySearchService>();
            serviceCollection.AddSingleton<IFulfillmentCenterSearchService, FulfillmentCenterSearchService>();
            serviceCollection.AddSingleton<IFulfillmentCenterService, FulfillmentCenterService>();
            serviceCollection.AddSingleton<InventoryExportImport>();
        }

        public void PostInitialize(IApplicationBuilder appBuilder)
        {
            _appBuilder = appBuilder;

            var settingsRegistrar = appBuilder.ApplicationServices.GetRequiredService<ISettingsRegistrar>();
            settingsRegistrar.RegisterSettings(ModuleConstants.Settings.AllSettings, ModuleInfo.Id);

            var permissionsProvider = appBuilder.ApplicationServices.GetRequiredService<IPermissionsRegistrar>();
            permissionsProvider.RegisterPermissions(ModuleConstants.Security.Permissions.AllPermissions.Select(x => new Permission() { GroupName = "Inventory", Name = x }).ToArray());

            //Force migrations
            using (var serviceScope = appBuilder.ApplicationServices.CreateScope())
            {
                var inventoryDbContext = serviceScope.ServiceProvider.GetRequiredService<InventoryDbContext>();
                inventoryDbContext.Database.MigrateIfNotApplied(MigrationName.GetUpdateV2MigrationName(ModuleInfo.Id));
                inventoryDbContext.Database.EnsureCreated();
                inventoryDbContext.Database.Migrate();
            }

            //Register product availability indexation 
            #region Search

            var productIndexingConfigurations = appBuilder.ApplicationServices.GetServices<IndexDocumentConfiguration>();
            if (productIndexingConfigurations != null)
            {
                var productAvaibilitySource = new IndexDocumentSource
                {
                    ChangesProvider = appBuilder.ApplicationServices.GetService<ProductAvailabilityChangesProvider>(),
                    DocumentBuilder = appBuilder.ApplicationServices.GetService<ProductAvailabilityDocumentBuilder>(),
                };

                foreach (var configuration in productIndexingConfigurations.Where(c => c.DocumentType == KnownDocumentTypes.Product))
                {
                    if (configuration.RelatedSources == null)
                    {
                        configuration.RelatedSources = new List<IndexDocumentSource>();
                    }
                    configuration.RelatedSources.Add(productAvaibilitySource);
                }
            }

            #endregion

            // enable polymorphic types in API controller methods
            var mvcJsonOptions = appBuilder.ApplicationServices.GetService<IOptions<MvcJsonOptions>>();
            mvcJsonOptions.Value.SerializerSettings.Converters.Add(new PolymorphicInventoryJsonConverter());
        }

        public void Uninstall()
        {
        }

        public async Task ExportAsync(Stream outStream, ExportImportOptions options, Action<ExportImportProgressInfo> progressCallback, ICancellationToken cancellationToken)
        {
            await _appBuilder.ApplicationServices.GetRequiredService<InventoryExportImport>().DoExportAsync(outStream, progressCallback, cancellationToken);
        }

        public async Task ImportAsync(Stream inputStream, ExportImportOptions options, Action<ExportImportProgressInfo> progressCallback, ICancellationToken cancellationToken)
        {
            await _appBuilder.ApplicationServices.GetRequiredService<InventoryExportImport>().DoImportAsync(inputStream, progressCallback, cancellationToken);
        }
    }
}
