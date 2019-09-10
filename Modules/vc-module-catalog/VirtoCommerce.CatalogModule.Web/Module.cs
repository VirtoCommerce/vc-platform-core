using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using VirtoCommerce.CatalogModule.Core;
using VirtoCommerce.CatalogModule.Core.Events;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.CatalogModule.Core.Model.Export;
using VirtoCommerce.CatalogModule.Core.Model.OutlinePart;
using VirtoCommerce.CatalogModule.Core.Search;
using VirtoCommerce.CatalogModule.Core.Services;
using VirtoCommerce.CatalogModule.Data.ExportImport;
using VirtoCommerce.CatalogModule.Data.Handlers;
using VirtoCommerce.CatalogModule.Data.Repositories;
using VirtoCommerce.CatalogModule.Data.Search;
using VirtoCommerce.CatalogModule.Data.Search.BrowseFilters;
using VirtoCommerce.CatalogModule.Data.Search.Indexing;
using VirtoCommerce.CatalogModule.Data.Services;
using VirtoCommerce.CatalogModule.Data.Validation;
using VirtoCommerce.CatalogModule.Web.Authorization;
using VirtoCommerce.CatalogModule.Web.JsonConverters;
using VirtoCommerce.CoreModule.Core.Seo;
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
using VirtoCommerce.SearchModule.Core.Model;
using VirtoCommerce.SearchModule.Core.Services;
using AuthorizationOptions = Microsoft.AspNetCore.Authorization.AuthorizationOptions;

namespace VirtoCommerce.CatalogModule.Web
{
    public class Module : IModule, IExportSupport, IImportSupport
    {
        private IApplicationBuilder _appBuilder;

        public ManifestModuleInfo ModuleInfo { get; set; }

        public void Initialize(IServiceCollection serviceCollection)
        {
            var configuration = serviceCollection.BuildServiceProvider().GetRequiredService<IConfiguration>();
            serviceCollection.AddTransient<ICatalogRepository, CatalogRepositoryImpl>();
            var connectionString = configuration.GetConnectionString("VirtoCommerce.Catalog") ?? configuration.GetConnectionString("VirtoCommerce");
            serviceCollection.AddDbContext<CatalogDbContext>(options => options.UseSqlServer(connectionString));
            serviceCollection.AddTransient<Func<ICatalogRepository>>(provider => () => provider.CreateScope().ServiceProvider.GetRequiredService<ICatalogRepository>());

            serviceCollection.AddTransient<IProductSearchService, ProductSearchService>();
            serviceCollection.AddTransient<ICategorySearchService, CategorySearchService>();

            serviceCollection.AddTransient<ICatalogService, CatalogService>();
            serviceCollection.AddTransient<ICatalogSearchService, CatalogSearchService>();
            serviceCollection.AddTransient<IListEntrySearchService, ListEntrySearchService>();

            serviceCollection.AddTransient<ICategoryService, CategoryService>();
            serviceCollection.AddTransient<ICategoryIndexedSearchService, CategoryIndexedSearchService>();

            serviceCollection.AddTransient<IItemService, ItemService>();
            serviceCollection.AddTransient<IProductIndexedSearchService, ProductIndexedSearchService>();
            serviceCollection.AddTransient<IAssociationService, AssociationService>();

            serviceCollection.AddTransient<IAggregationConverter, AggregationConverter>();
            serviceCollection.AddTransient<IBrowseFilterService, BrowseFilterService>();
            serviceCollection.AddTransient<ITermFilterBuilder, TermFilterBuilder>();

            serviceCollection.AddTransient<ISearchRequestBuilder, ProductSearchRequestBuilder>();
            serviceCollection.AddTransient<ISearchRequestBuilder, CategorySearchRequestBuilder>();

            serviceCollection.AddTransient<IPropertyService, PropertyService>();
            serviceCollection.AddTransient<IPropertySearchService, PropertySearchService>();
            serviceCollection.AddTransient<IPropertyDictionaryItemService, PropertyDictionaryItemService>();
            serviceCollection.AddTransient<IPropertyDictionaryItemSearchService, PropertyDictionaryItemSearchService>();
            serviceCollection.AddTransient<IProductAssociationSearchService, ProductAssociationSearchService>();
            serviceCollection.AddTransient<IOutlineService, OutlineService>();
            serviceCollection.AddTransient<ISkuGenerator, DefaultSkuGenerator>();

            serviceCollection.AddTransient<LogChangesChangedEventHandler>();

            serviceCollection.AddTransient<ISeoBySlugResolver, SeoBySlugResolver>();

            PropertyValueValidator PropertyValueValidatorFactory(PropertyValidationRule rule) => new PropertyValueValidator(rule);
            serviceCollection.AddSingleton((Func<PropertyValidationRule, PropertyValueValidator>)PropertyValueValidatorFactory);
            serviceCollection.AddTransient<AbstractValidator<IHasProperties>, HasPropertiesValidator>();

            serviceCollection.AddTransient<CatalogExportImport>();

            serviceCollection.AddTransient<IOutlinePartResolver>(provider =>
            {
                var settingsManager = provider.GetService<ISettingsManager>();
                if (settingsManager.GetValue(ModuleConstants.Settings.General.CodesInOutline.Name, false))
                {
                    return new CodeOutlinePartResolver();
                }
                else
                {
                    return new IdOutlinePartResolver();
                }
            });

            serviceCollection.AddTransient<ProductDocumentChangesProvider>();
            serviceCollection.AddTransient<ProductDocumentBuilder>();
            serviceCollection.AddTransient<CategoryDocumentChangesProvider>();
            serviceCollection.AddTransient<CategoryDocumentBuilder>();

            // Product indexing configuration
            serviceCollection.AddSingleton(provider => new IndexDocumentConfiguration
            {
                DocumentType = KnownDocumentTypes.Product,
                DocumentSource = new IndexDocumentSource
                {
                    ChangesProvider = provider.GetService<ProductDocumentChangesProvider>(),
                    DocumentBuilder = provider.GetService<ProductDocumentBuilder>(),
                },
            });

            // Category indexing configuration
            serviceCollection.AddSingleton(provider => new IndexDocumentConfiguration
            {
                DocumentType = KnownDocumentTypes.Category,
                DocumentSource = new IndexDocumentSource
                {
                    ChangesProvider = provider.GetService<CategoryDocumentChangesProvider>(),
                    DocumentBuilder = provider.GetService<CategoryDocumentBuilder>(),
                },
            });

            serviceCollection.AddTransient<IAuthorizationHandler, CatalogAuthorizationHandler>();

            serviceCollection.AddTransient<ICatalogExportPagedDataSourceFactory, CatalogExportPagedDataSourceFactory>();


            #region Add Authorization Policy for GenericExport

            var requirements = new IAuthorizationRequirement[]
            {
                new PermissionAuthorizationRequirement(ModuleConstants.Security.Permissions.Export),
                new CatalogAuthorizationRequirement(ModuleConstants.Security.Permissions.Read)
            };

            var exportPolicy = new AuthorizationPolicyBuilder()
                .AddRequirements(requirements)
                .Build();

            serviceCollection.Configure<AuthorizationOptions>(configure =>
            {
                configure.AddPolicy(typeof(ExportableProduct).FullName + "ExportDataPolicy", exportPolicy);
                configure.AddPolicy(typeof(ExportableCatalogFull).FullName + "ExportDataPolicy", exportPolicy);
            });

            #endregion
        }

        public void PostInitialize(IApplicationBuilder appBuilder)
        {
            _appBuilder = appBuilder;

            var settingsRegistrar = appBuilder.ApplicationServices.GetRequiredService<ISettingsRegistrar>();
            settingsRegistrar.RegisterSettings(ModuleConstants.Settings.AllSettings, ModuleInfo.Id);

            //Register module permissions
            var permissionsProvider = appBuilder.ApplicationServices.GetRequiredService<IPermissionsRegistrar>();
            permissionsProvider.RegisterPermissions(ModuleConstants.Security.Permissions.AllPermissions.Select(x => new Permission() { GroupName = "Catalog", Name = x }).ToArray());


            //Register Permission scopes
            AbstractTypeFactory<PermissionScope>.RegisterType<SelectedCatalogScope>();
            permissionsProvider.WithAvailabeScopesForPermissions(new[] {
                                                                        ModuleConstants.Security.Permissions.Read,
                                                                        ModuleConstants.Security.Permissions.Update,
                                                                        ModuleConstants.Security.Permissions.Delete,
                                                                         }, new SelectedCatalogScope());

            var mvcJsonOptions = appBuilder.ApplicationServices.GetService<IOptions<MvcJsonOptions>>();
            mvcJsonOptions.Value.SerializerSettings.Converters.Add(new SearchCriteriaJsonConverter());

            var inProcessBus = appBuilder.ApplicationServices.GetService<IHandlerRegistrar>();
            inProcessBus.RegisterHandler<ProductChangedEvent>(async (message, token) => await appBuilder.ApplicationServices.GetService<LogChangesChangedEventHandler>().Handle(message));
            inProcessBus.RegisterHandler<CategoryChangedEvent>(async (message, token) => await appBuilder.ApplicationServices.GetService<LogChangesChangedEventHandler>().Handle(message));


            //Force migrations
            using (var serviceScope = appBuilder.ApplicationServices.CreateScope())
            {
                var catalogDbContext = serviceScope.ServiceProvider.GetRequiredService<CatalogDbContext>();
                catalogDbContext.Database.MigrateIfNotApplied(MigrationName.GetUpdateV2MigrationName(ModuleInfo.Id));
                catalogDbContext.Database.EnsureCreated();
                catalogDbContext.Database.Migrate();
            }

            #region Register types for generic Export

            var registrar = appBuilder.ApplicationServices.GetService<IKnownExportTypesRegistrar>();

            registrar.RegisterType(
                ExportedTypeDefinitionBuilder.Build<ExportableProduct, ProductExportDataQuery>()
                    .WithDataSourceFactory(appBuilder.ApplicationServices.GetService<ICatalogExportPagedDataSourceFactory>())
                    .WithMetadata(typeof(ExportableProduct).GetPropertyNames(
                        nameof(ExportableProduct.Properties),
                        $"{nameof(ExportableProduct.Properties)}.{nameof(Property.Values)}",
                        $"{nameof(ExportableProduct.Properties)}.{nameof(Property.Attributes)}",
                        $"{nameof(ExportableProduct.Properties)}.{nameof(Property.DisplayNames)}",
                        $"{nameof(ExportableProduct.Properties)}.{nameof(Property.ValidationRules)}",
                        nameof(ExportableProduct.Assets),
                        nameof(ExportableProduct.Links),
                        nameof(ExportableProduct.SeoInfos),
                        nameof(ExportableProduct.Reviews),
                        nameof(ExportableProduct.Associations),
                        nameof(ExportableProduct.ReferencedAssociations),
                        nameof(ExportableProduct.Outlines),
                        nameof(ExportableProduct.Images)))
                    .WithTabularMetadata(typeof(ExportableProduct).GetPropertyNames()));

            registrar.RegisterType(
                ExportedTypeDefinitionBuilder.Build<ExportableCatalogFull, CatalogFullExportDataQuery>()
                    .WithDataSourceFactory(appBuilder.ApplicationServices.GetService<ICatalogExportPagedDataSourceFactory>())
                    .WithMetadata(new ExportedTypeMetadata { PropertyInfos = Array.Empty<ExportedTypePropertyInfo>() }));

            #endregion
        }

        public void Uninstall()
        {
        }

        public async Task ExportAsync(Stream outStream, ExportImportOptions options, Action<ExportImportProgressInfo> progressCallback,
            ICancellationToken cancellationToken)
        {
            await _appBuilder.ApplicationServices.GetRequiredService<CatalogExportImport>().DoExportAsync(outStream, options,
                progressCallback, cancellationToken);
        }

        public async Task ImportAsync(Stream inputStream, ExportImportOptions options, Action<ExportImportProgressInfo> progressCallback,
            ICancellationToken cancellationToken)
        {
            await _appBuilder.ApplicationServices.GetRequiredService<CatalogExportImport>().DoImportAsync(inputStream, options,
                progressCallback, cancellationToken);
        }
    }
}
