using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using VirtoCommerce.Domain.Search;
using VirtoCommerce.InventoryModule.Core.Services;
using VirtoCommerce.InventoryModule.Data.Repositories;
using VirtoCommerce.InventoryModule.Data.Search.Indexing;
using VirtoCommerce.InventoryModule.Data.Services;
using VirtoCommerce.InventoryModule.Web.JsonConverters;
using VirtoCommerce.Platform.Core.Modularity;

namespace VirtoCommerce.InventoryModule.Web
{
    public class Module : IModule
    {
        public ManifestModuleInfo ModuleInfo { get; set; }
        public void Initialize(IServiceCollection serviceCollection)
        {
            var configuration = serviceCollection.BuildServiceProvider().GetRequiredService<IConfiguration>();
            serviceCollection.AddTransient<IInventoryRepository, InventoryRepositoryImpl>();
            serviceCollection.AddDbContext<InventoryDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("VirtoCommerce")));
            serviceCollection.AddSingleton<Func<IInventoryRepository>>(provider => () => provider.CreateScope().ServiceProvider.GetRequiredService<IInventoryRepository>());
            serviceCollection.AddSingleton<IInventoryService, InventoryServiceImpl>();
            serviceCollection.AddSingleton<IInventorySearchService, InventorySearchService>();
            serviceCollection.AddSingleton<IFulfillmentCenterSearchService, FulfillmentCenterSearchService>();
            serviceCollection.AddSingleton<IFulfillmentCenterService, FulfillmentCenterService>();
        }

        public void PostInitialize(IServiceProvider serviceProvider)
        {
            //Register product availability indexation 
            #region Search

            var productIndexingConfigurations = serviceProvider.GetService<IndexDocumentConfiguration[]>();
            if (productIndexingConfigurations != null)
            {
                var productAvaibilitySource = new IndexDocumentSource
                {
                    ChangesProvider = serviceProvider.GetService<ProductAvailabilityChangesProvider>(),
                    DocumentBuilder = serviceProvider.GetService<ProductAvailabilityDocumentBuilder>(),
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
            var mvcJsonOptions = serviceProvider.GetService<IOptions<MvcJsonOptions>>();
            mvcJsonOptions.Value.SerializerSettings.Converters.Add(new PolymorphicInventoryJsonConverter());
        }

        public void Uninstall()
        {
        }
    }
}
