using System;
using System.Linq;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.CatalogModule.Core2;
using VirtoCommerce.CatalogModule.Core2.Model;
using VirtoCommerce.CatalogModule.Core2.Services;
using VirtoCommerce.CatalogModule.Data2.Repositories;
using VirtoCommerce.CatalogModule.Data2.Services;
using VirtoCommerce.CatalogModule.Data2.Validation;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.Platform.Core.Settings;

namespace VirtoCommerce.CatalogModule.Web2
{
    public class Module : IModule
    {
        public ManifestModuleInfo ModuleInfo { get; set; }

        public void Initialize(IServiceCollection serviceCollection)
        {
            var configuration = serviceCollection.BuildServiceProvider().GetRequiredService<IConfiguration>();
            serviceCollection.AddTransient<ICatalogRepository, CatalogRepository>();
            serviceCollection.AddDbContext<CatalogDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("VirtoCommerce")));
            serviceCollection.AddSingleton<Func<ICatalogRepository>>(provider => () => provider.CreateScope().ServiceProvider.GetRequiredService<ICatalogRepository>());
            serviceCollection.AddSingleton<ICatalogService, CatalogService>();
            serviceCollection.AddSingleton<ICategoryService, CategoryService>();
            serviceCollection.AddSingleton<IOutlineService, OutlineService>();
            serviceCollection.AddSingleton<IItemService, ItemService>();
            serviceCollection.AddSingleton<IPropertyService, PropertyService>();
            serviceCollection.AddSingleton<IPropertySearchService, PropertySearchService>();
            serviceCollection.AddSingleton<ISkuGenerator, DefaultSkuGenerator>();
            serviceCollection.AddSingleton<IListEntrySearchService, ListEntrySearchService>();

            Func<PropertyValidationRule, PropertyValueValidator> propertyValueValidatorFactory =
              rule => new PropertyValueValidator(rule);

            serviceCollection.AddSingleton(propertyValueValidatorFactory);
            serviceCollection.AddSingleton<AbstractValidator<IHasProperties>, HasPropertiesValidator>();
        }

        public void PostInitialize(IApplicationBuilder appBuilder)
        {
            //Register module settings
            var settingsRegistrar = appBuilder.ApplicationServices.GetRequiredService<ISettingsRegistrar>();
            settingsRegistrar.RegisterSettings(ModuleConstants.Settings.AllSettings, ModuleInfo.Id);

            //Register module permissions
            var permissionsProvider = appBuilder.ApplicationServices.GetRequiredService<IPermissionsRegistrar>();
            permissionsProvider.RegisterPermissions(ModuleConstants.Security.Permissions.AllPermissions.Select(x => new Permission() { GroupName = "Catalog", Name = x }).ToArray());

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
    }
}
