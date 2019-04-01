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
using VirtoCommerce.CoreModule.Core.Conditions;
using VirtoCommerce.CoreModule.Core.Conditions.Browse;
using VirtoCommerce.CoreModule.Core.Conditions.GeoConditions;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.ExportImport;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.PricingModule.Core;
using VirtoCommerce.PricingModule.Core.Model;
using VirtoCommerce.PricingModule.Core.Model.Conditions;
using VirtoCommerce.PricingModule.Core.Services;
using VirtoCommerce.PricingModule.Data.ExportImport;
using VirtoCommerce.PricingModule.Data.Repositories;
using VirtoCommerce.PricingModule.Data.Search;
using VirtoCommerce.PricingModule.Data.Services;
using VirtoCommerce.PricingModule.Web.JsonConverters;
using VirtoCommerce.SearchModule.Core.Model;

namespace VirtoCommerce.PricingModule.Web
{
    public class Module : IModule, IExportSupport, IImportSupport
    {
        private IApplicationBuilder _applicationBuilder;

        #region IModule Members

        public ManifestModuleInfo ModuleInfo { get; set; }

        public void Initialize(IServiceCollection serviceCollection)
        {
            var configuration = serviceCollection.BuildServiceProvider().GetRequiredService<IConfiguration>();
            var connectionString = configuration.GetConnectionString("VirtoCommerce.Pricing") ?? configuration.GetConnectionString("VirtoCommerce");
            serviceCollection.AddDbContext<PricingDbContext>(options => options.UseSqlServer(connectionString));
            serviceCollection.AddTransient<IPricingRepository, PricingRepositoryImpl>();
            serviceCollection.AddSingleton<Func<IPricingRepository>>(provider => () => provider.CreateScope().ServiceProvider.GetRequiredService<IPricingRepository>());

            serviceCollection.AddTransient<IPricingService, PricingServiceImpl>();
            serviceCollection.AddTransient<IPricingSearchService, PricingSearchServiceImpl>();
            serviceCollection.AddSingleton<IPricingExtensionManager, DefaultPricingExtensionManagerImpl>();
            serviceCollection.AddSingleton<PricingExportImport>();
            serviceCollection.AddSingleton<PolymorphicPricingJsonConverter>();
            serviceCollection.AddTransient<ProductPriceDocumentChangesProvider>();
            serviceCollection.AddTransient<ProductPriceDocumentBuilder>();
        }

        public void PostInitialize(IApplicationBuilder appBuilder)
        {
            _applicationBuilder = appBuilder;

            var settingsRegistrar = appBuilder.ApplicationServices.GetRequiredService<ISettingsRegistrar>();
            settingsRegistrar.RegisterSettings(ModuleConstants.Settings.AllSettings, ModuleInfo.Id);

            var modulePermissions = ModuleConstants.Security.Permissions.AllPermissions.Select(p => new Permission
            {
                Name = p,
                GroupName = "Pricing",
                ModuleId = ModuleInfo.Id
            }).ToArray();
            var permissionsRegistrar = appBuilder.ApplicationServices.GetRequiredService<IPermissionsRegistrar>();
            permissionsRegistrar.RegisterPermissions(modulePermissions);

            using (var serviceScope = appBuilder.ApplicationServices.CreateScope())
            {
                var dbContext = serviceScope.ServiceProvider.GetRequiredService<PricingDbContext>();
                dbContext.Database.EnsureCreated();
                dbContext.Database.Migrate();
            }

            // Next lines allow to use polymorph types in API controller methods
            var mvcJsonOptions = appBuilder.ApplicationServices.GetService<IOptions<MvcJsonOptions>>();
            mvcJsonOptions.Value.SerializerSettings.Converters.Add(appBuilder.ApplicationServices.GetService<PolymorphicPricingJsonConverter>());

            // Add price document source to the product indexing configuration
            var productIndexingConfigurations = appBuilder.ApplicationServices.GetServices<IndexDocumentConfiguration>();
            if (productIndexingConfigurations != null)
            {
                var productPriceDocumentSource = new IndexDocumentSource
                {
                    ChangesProvider = appBuilder.ApplicationServices.GetService<ProductPriceDocumentChangesProvider>(),
                    DocumentBuilder = appBuilder.ApplicationServices.GetService<ProductPriceDocumentBuilder>()
                };

                foreach (var configuration in productIndexingConfigurations.Where(c => c.DocumentType == KnownDocumentTypes.Product))
                {
                    if (configuration.RelatedSources == null)
                    {
                        configuration.RelatedSources = new List<IndexDocumentSource>();
                    }

                    configuration.RelatedSources.Add(productPriceDocumentSource);
                }
            }

            //Pricing expression
            AbstractTypeFactory<IConditionTree>.RegisterType<PriceConditionTree>();
            AbstractTypeFactory<IConditionTree>.RegisterType<BlockPricingCondition>();

            var pricingExtensionManager = appBuilder.ApplicationServices.GetRequiredService<IPricingExtensionManager>();
            pricingExtensionManager.PriceConditionTree = new PriceConditionTree
            {
                Children = new List<IConditionTree>() { GetPricingDynamicExpression() }
            };
        }

        public void Uninstall()
        {
        }

        #endregion

        #region ISupportExportImportModule Members

        public async Task ExportAsync(Stream outStream, ExportImportOptions options, Action<ExportImportProgressInfo> progressCallback,
            ICancellationToken cancellationToken)
        {
            var exportJob = _applicationBuilder.ApplicationServices.GetRequiredService<PricingExportImport>();
            await exportJob.DoExportAsync(outStream, progressCallback, cancellationToken);
        }

        public async Task ImportAsync(Stream inputStream, ExportImportOptions options, Action<ExportImportProgressInfo> progressCallback,
            ICancellationToken cancellationToken)
        {
            var importJob = _applicationBuilder.ApplicationServices.GetRequiredService<PricingExportImport>();
            await importJob.DoImportAsync(inputStream, progressCallback, cancellationToken);
        }

        #endregion

        private static IConditionTree GetPricingDynamicExpression()
        {
            var conditions = new List<IConditionTree>
            {
                new ConditionGeoTimeZone(), new ConditionGeoZipCode(), new ConditionStoreSearchedPhrase(), new ConditionAgeIs(), new ConditionGenderIs(),
                new ConditionGeoCity(), new ConditionGeoCountry(), new ConditionGeoState(), new ConditionLanguageIs(), new UserGroupsContainsCondition()
            };
            var rootBlock = new BlockPricingCondition { AvailableChildren = conditions };

            return rootBlock;
        }
    }
}
