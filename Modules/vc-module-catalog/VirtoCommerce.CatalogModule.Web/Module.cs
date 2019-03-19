using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using VirtoCommerce.CatalogModule.Core;
using VirtoCommerce.CatalogModule.Core.Events;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.CatalogModule.Core.Search;
using VirtoCommerce.CatalogModule.Core.Services;
using VirtoCommerce.CatalogModule.Data.ExportImport;
using VirtoCommerce.CatalogModule.Data.Handlers;
using VirtoCommerce.CatalogModule.Data.Repositories;
using VirtoCommerce.CatalogModule.Data.Search;
using VirtoCommerce.CatalogModule.Data.Search.BrowseFilters;
using VirtoCommerce.CatalogModule.Data.Services;
using VirtoCommerce.CatalogModule.Data.Services.OutlineParts;
using VirtoCommerce.CatalogModule.Data.Validation;
using VirtoCommerce.CatalogModule.Web.JsonConverters;
using VirtoCommerce.Platform.Core.Bus;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.ExportImport;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.SearchModule.Core.Services;

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
            serviceCollection.AddSingleton<Func<ICatalogRepository>>(provider => () => provider.CreateScope().ServiceProvider.GetRequiredService<ICatalogRepository>());

            serviceCollection.AddSingleton<ICatalogService, CatalogServiceImpl>();
            serviceCollection.AddSingleton<ICatalogSearchService, CatalogSearchServiceDecorator>();
            serviceCollection.AddSingleton<CatalogSearchServiceImpl>();

            serviceCollection.AddSingleton<ICategoryService, CategoryServiceImpl>();
            serviceCollection.AddSingleton<ICategorySearchService, CategorySearchService>();

            serviceCollection.AddSingleton<IItemService, ItemServiceImpl>();
            serviceCollection.AddSingleton<IProductSearchService, ProductSearchService>();

            serviceCollection.AddSingleton<IAggregationConverter, AggregationConverter>();
            serviceCollection.AddSingleton<IBrowseFilterService, BrowseFilterService>();
            serviceCollection.AddSingleton<ITermFilterBuilder, TermFilterBuilder>();

            serviceCollection.AddSingleton<ISearchRequestBuilder, ProductSearchRequestBuilder>();
            serviceCollection.AddSingleton<ISearchRequestBuilder, CategorySearchRequestBuilder>();

            serviceCollection.AddSingleton<IPropertyService, PropertyServiceImpl>();
            serviceCollection.AddSingleton<IProperyDictionaryItemService, PropertyDictionaryItemService>();
            serviceCollection.AddSingleton<IProperyDictionaryItemSearchService, ProperyDictionaryItemSearchService>();
            serviceCollection.AddSingleton<IProductAssociationSearchService, ProductAssociationSearchService>();
            serviceCollection.AddSingleton<IOutlineService, OutlineService>();
            serviceCollection.AddSingleton<ISkuGenerator, DefaultSkuGenerator>();

            PropertyValueValidator PropertyValueValidatorFactory(PropertyValidationRule rule) => new PropertyValueValidator(rule);
            serviceCollection.AddSingleton((Func<PropertyValidationRule, PropertyValueValidator>)PropertyValueValidatorFactory);
            serviceCollection.AddSingleton<AbstractValidator<IHasProperties>, HasPropertiesValidator>();

            serviceCollection.AddSingleton<CatalogExportImport>();
            serviceCollection.AddSingleton<CategoryChangedEventHandler>();
            serviceCollection.AddSingleton<ProductChangedEventHandler>();

            var settingsRegistrar = serviceCollection.BuildServiceProvider().GetRequiredService<ISettingsRegistrar>();
            settingsRegistrar.RegisterSettings(ModuleConstants.Settings.AllSettings, ModuleInfo.Id);

            var settingManager = serviceCollection.BuildServiceProvider().GetService<ISettingsManager>();
            if (settingManager.GetValue(ModuleConstants.Settings.General.CodesInOutline.Name, false))
                serviceCollection.AddSingleton<IOutlinePartResolver, CodeOutlinePartResolver>();
            else
                serviceCollection.AddSingleton<IOutlinePartResolver, IdOutlinePartResolver>();
        }

        public void PostInitialize(IApplicationBuilder appBuilder)
        {
            _appBuilder = appBuilder;

            //Register module permissions
            var permissionsProvider = appBuilder.ApplicationServices.GetRequiredService<IPermissionsRegistrar>();
            permissionsProvider.RegisterPermissions(ModuleConstants.Security.Permissions.AllPermissions.Select(x => new Permission() { GroupName = "Catalog", Name = x }).ToArray());

            var mvcJsonOptions = appBuilder.ApplicationServices.GetService<IOptions<MvcJsonOptions>>();
            mvcJsonOptions.Value.SerializerSettings.Converters.Add(new SearchCriteriaJsonConverter());

            var inProcessBus = appBuilder.ApplicationServices.GetService<IHandlerRegistrar>();
            inProcessBus.RegisterHandler<CategoryChangedEvent>(async (message, token) => await appBuilder.ApplicationServices.GetService<CategoryChangedEventHandler>().Handle(message));
            inProcessBus.RegisterHandler<CategoryChangingEvent>(async (message, token) => await appBuilder.ApplicationServices.GetService<CategoryChangedEventHandler>().Handle(message));
            inProcessBus.RegisterHandler<ProductChangedEvent>(async (message, token) => await appBuilder.ApplicationServices.GetService<ProductChangedEventHandler>().Handle(message));
            inProcessBus.RegisterHandler<ProductChangingEvent>(async (message, token) => await appBuilder.ApplicationServices.GetService<ProductChangedEventHandler>().Handle(message));

            //Force migrations
            using (var serviceScope = appBuilder.ApplicationServices.CreateScope())
            {
                var catalogDbContext = serviceScope.ServiceProvider.GetRequiredService<CatalogDbContext>();
                catalogDbContext.Database.EnsureCreated();
                catalogDbContext.Database.Migrate();
            }
        }

        public void Uninstall()
        {
        }

        public Task ExportAsync(Stream outStream, ExportImportOptions options, Action<ExportImportProgressInfo> progressCallback,
            ICancellationToken cancellationToken)
        {
            return _appBuilder.ApplicationServices.GetRequiredService<CatalogExportImport>().DoExportAsync(outStream, options,
                progressCallback, cancellationToken);
        }

        public Task ImportAsync(Stream inputStream, ExportImportOptions options, Action<ExportImportProgressInfo> progressCallback,
            ICancellationToken cancellationToken)
        {
            return _appBuilder.ApplicationServices.GetRequiredService<CatalogExportImport>().DoImportAsync(inputStream, options,
                progressCallback, cancellationToken);
        }
    }
}
