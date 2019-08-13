using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using VirtoCommerce.CoreModule.Core.Conditions;
using VirtoCommerce.CoreModule.Core.Conditions.Browse;
using VirtoCommerce.CoreModule.Core.Conditions.GeoConditions;
using VirtoCommerce.ExportModule.Core.Model;
using VirtoCommerce.ExportModule.Core.Services;
using VirtoCommerce.ExportModule.Data.Extensions;
using VirtoCommerce.ExportModule.Data.Services;
using VirtoCommerce.Platform.Core.Bus;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.ExportImport;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.Platform.Data.Extensions;
using VirtoCommerce.Platform.Security.Authorization;
using VirtoCommerce.PricingModule.Core;
using VirtoCommerce.PricingModule.Core.Events;
using VirtoCommerce.PricingModule.Core.Model;
using VirtoCommerce.PricingModule.Core.Model.Conditions;
using VirtoCommerce.PricingModule.Core.Services;
using VirtoCommerce.PricingModule.Data.ExportImport;
using VirtoCommerce.PricingModule.Data.ExportImport.Converters;
using VirtoCommerce.PricingModule.Data.Handlers;
using VirtoCommerce.PricingModule.Data.Repositories;
using VirtoCommerce.PricingModule.Data.Search;
using VirtoCommerce.PricingModule.Data.Services;
using VirtoCommerce.PricingModule.Web.JsonConverters;
using VirtoCommerce.SearchModule.Core.Model;
using VirtoCommerce.SearchModule.Core.Services;

namespace VirtoCommerce.PricingModule.Web
{
    public class Module : IModule, IExportSupport, IImportSupport
    {
        private IApplicationBuilder _applicationBuilder;
        private ServiceProvider _serviceProvider;

        #region IModule Members

        public ManifestModuleInfo ModuleInfo { get; set; }

        public void Initialize(IServiceCollection serviceCollection)
        {
            _serviceProvider = serviceCollection.BuildServiceProvider();
            var configuration = _serviceProvider.GetRequiredService<IConfiguration>();
            var connectionString = configuration.GetConnectionString("VirtoCommerce.Pricing") ?? configuration.GetConnectionString("VirtoCommerce");
            serviceCollection.AddDbContext<PricingDbContext>(options => options.UseSqlServer(connectionString));
            serviceCollection.AddTransient<IPricingRepository, PricingRepositoryImpl>();
            serviceCollection.AddTransient<Func<IPricingRepository>>(provider => () => provider.CreateScope().ServiceProvider.GetRequiredService<IPricingRepository>());

            serviceCollection.AddTransient<IPricingService, PricingServiceImpl>();
            serviceCollection.AddSingleton<Func<IPricingService>>(provider => () => provider.CreateScope().ServiceProvider.GetRequiredService<IPricingService>());
            serviceCollection.AddTransient<IPricingSearchService, PricingSearchServiceImpl>();
            serviceCollection.AddTransient<PricingExportImport>();
            serviceCollection.AddTransient<PolymorphicPricingJsonConverter>();
            serviceCollection.AddTransient<ProductPriceDocumentChangesProvider>();
            serviceCollection.AddTransient<ProductPriceDocumentBuilder>();
            serviceCollection.AddTransient<LogChangesChangedEventHandler>();

            serviceCollection.AddSingleton<IPricingExtensionManager, DefaultPricingExtensionManagerImpl>();

            serviceCollection.AddTransient<PriceExportPagedDataSource>(); // Adding as scoped, because of used services (UserManager, PrincipalFactory) scoped too
            serviceCollection.AddTransient<Func<ExportDataQuery, PriceExportPagedDataSource>>(provider =>
                (exportDataQuery) =>
                {
                    var datasource = provider.CreateScope().ServiceProvider.GetRequiredService<PriceExportPagedDataSource>();
                    datasource.DataQuery = (PriceExportDataQuery)exportDataQuery;
                    return datasource;
                });

            serviceCollection.AddTransient<PricelistExportPagedDataSource>();
            serviceCollection.AddTransient<Func<ExportDataQuery, PricelistExportPagedDataSource>>(provider =>
                (exportDataQuery) =>
                {
                    var datasource = provider.CreateScope().ServiceProvider.GetRequiredService<PricelistExportPagedDataSource>();
                    datasource.DataQuery = (PricelistExportDataQuery)exportDataQuery;
                    return datasource;
                });

            serviceCollection.AddTransient<PricelistAssignmentExportPagedDataSource>();
            serviceCollection.AddTransient<Func<ExportDataQuery, PricelistAssignmentExportPagedDataSource>>(provider =>
                (exportDataQuery) =>
                {
                    var datasource = provider.CreateScope().ServiceProvider.GetRequiredService<PricelistAssignmentExportPagedDataSource>();
                    datasource.DataQuery = (PricelistAssignmentExportDataQuery)exportDataQuery;
                    return datasource;
                });
            var requirements = new IAuthorizationRequirement[]
            {
                new PermissionAuthorizationRequirement(ModuleConstants.Security.Permissions.Export), new PermissionAuthorizationRequirement(ModuleConstants.Security.Permissions.Read)
            };

            var exportPolicy = new AuthorizationPolicyBuilder()
                .AddRequirements(requirements)
                .Build();

            serviceCollection.Configure<Microsoft.AspNetCore.Authorization.AuthorizationOptions>(configure =>
            {
                configure.AddPolicy(typeof(ExportablePricelist).FullName + "FullDataExportDataPolicy", exportPolicy);
                configure.AddPolicy(typeof(ExportablePricelist).FullName + "ExportDataPolicy", exportPolicy);
                configure.AddPolicy(typeof(ExportablePrice).FullName + "ExportDataPolicy", exportPolicy);
                configure.AddPolicy(typeof(ExportablePricelistAssignment).FullName + "ExportDataPolicy", exportPolicy);
            });
        }

        public void PostInitialize(IApplicationBuilder appBuilder)
        {
            _applicationBuilder = appBuilder;
            var settingsManager = _serviceProvider.GetService<ISettingsManager>();

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
                dbContext.Database.MigrateIfNotApplied(MigrationName.GetUpdateV2MigrationName(ModuleInfo.Id));
                dbContext.Database.EnsureCreated();
                dbContext.Database.Migrate();
            }

            // Next lines allow to use polymorph types in API controller methods
            var mvcJsonOptions = appBuilder.ApplicationServices.GetService<IOptions<MvcJsonOptions>>();
            mvcJsonOptions.Value.SerializerSettings.Converters.Add(appBuilder.ApplicationServices.GetService<PolymorphicPricingJsonConverter>());

            var priceIndexingEnabled = settingsManager.GetValue(ModuleConstants.Settings.General.PricingIndexing.Name, true);
            if (priceIndexingEnabled)
            {
                // Add price document source to the product indexing configuration
                var productIndexingConfigurations = _serviceProvider.GetService<IndexDocumentConfiguration[]>();

                if (productIndexingConfigurations != null)
                {
                    var productPriceDocumentSource = new IndexDocumentSource
                    {
                        ChangesProvider = _serviceProvider.GetService<IIndexDocumentChangesProvider>(),
                        DocumentBuilder = _serviceProvider.GetService<ProductPriceDocumentBuilder>(),
                    };

                    foreach (var configuration in productIndexingConfigurations.Where(c =>
                        c.DocumentType == KnownDocumentTypes.Product))
                    {
                        if (configuration.RelatedSources == null)
                        {
                            configuration.RelatedSources = new List<IndexDocumentSource>();
                        }

                        configuration.RelatedSources.Add(productPriceDocumentSource);
                    }
                }
            }

            var inProcessBus = appBuilder.ApplicationServices.GetService<IHandlerRegistrar>();
            inProcessBus.RegisterHandler<PriceChangedEvent>(async (message, token) => await appBuilder.ApplicationServices.GetService<LogChangesChangedEventHandler>().Handle(message));

            //Pricing expression
            AbstractTypeFactory<IConditionTree>.RegisterType<PriceConditionTree>();
            AbstractTypeFactory<IConditionTree>.RegisterType<BlockPricingCondition>();

            var pricingExtensionManager = appBuilder.ApplicationServices.GetRequiredService<IPricingExtensionManager>();
            pricingExtensionManager.PriceConditionTree = new PriceConditionTree
            {
                Children = new List<IConditionTree>() { GetPricingDynamicExpression() }
            };

            var registrar = appBuilder.ApplicationServices.GetService<IKnownExportTypesRegistrar>();
            var priceExportPagedDataSourceFactory = appBuilder.ApplicationServices.GetService<Func<ExportDataQuery, PriceExportPagedDataSource>>();
            var pricelistExportPagedDataSourceFactory = appBuilder.ApplicationServices.GetService<Func<ExportDataQuery, PricelistExportPagedDataSource>>();
            var pricelistAssignmentExportPagedDataSourceFactory = appBuilder.ApplicationServices.GetService<Func<ExportDataQuery, PricelistAssignmentExportPagedDataSource>>();

            registrar.RegisterType(
                 ExportedTypeDefinitionBuilder.Build<ExportablePrice, PriceExportDataQuery>()
                    .WithDataSourceFactory(dataQuery => priceExportPagedDataSourceFactory(dataQuery))
                    .WithMetadata(typeof(ExportablePrice).GetPropertyNames(false))
                    .WithTabularDataConverter(new TabularPriceDataConverter())
                    .WithTabularMetadata(typeof(TabularPrice).GetPropertyNames(false))
                    .ExportedTypeDefinition);

            registrar.RegisterType(
                 ExportedTypeDefinitionBuilder.Build<ExportablePricelist, PricelistExportDataQuery>()
                    .WithDataSourceFactory(dataQuery => pricelistExportPagedDataSourceFactory(dataQuery))
                    .WithMetadata(typeof(ExportablePricelist).GetPropertyNames(false))
                    .WithTabularDataConverter(new TabularPricelistDataConverter())
                    .WithTabularMetadata(typeof(TabularPricelist).GetPropertyNames(false))
                    .ExportedTypeDefinition);

            registrar.RegisterType(
                 ExportedTypeDefinitionBuilder.Build<ExportablePricelistAssignment, PricelistAssignmentExportDataQuery>()
                    .WithDataSourceFactory(dataQuery => pricelistAssignmentExportPagedDataSourceFactory(dataQuery))
                    .WithMetadata(typeof(ExportablePricelistAssignment).GetPropertyNames(false))
                    .WithTabularDataConverter(new TabularPricelistAssignmentDataConverter())
                    .WithTabularMetadata(typeof(TabularPricelistAssignment).GetPropertyNames(false))
                    .ExportedTypeDefinition);

            //registrar.RegisterType(
            //    new ExportedTypeDefinitionBuilder($@"{typeof(ExportablePricelist).FullName}FullData", typeof(ExportablePricelist).Namespace, typeof(PricelistExportDataQuery).Name)
            //        .WithDataSourceFactory(dataQuery => pricelistExportPagedDataSourceFactory(dataQuery))
            //        .WithMetadata(typeof(ExportablePricelist).GetPropertyNames(true))
            //        .ExportedTypeDefinition);

            AbstractTypeFactory<ExportDataQuery>.RegisterType<PriceExportDataQuery>();
            AbstractTypeFactory<ExportDataQuery>.RegisterType<PricelistAssignmentExportDataQuery>();
            AbstractTypeFactory<ExportDataQuery>.RegisterType<PricelistExportDataQuery>();
        }

        public void Uninstall()
        {
            // no need to perform actions for now (Comment to remove Sonar warning)
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
