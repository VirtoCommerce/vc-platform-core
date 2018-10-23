using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using VirtoCommerce.CartModule.Core;
using VirtoCommerce.CartModule.Core.Events;
using VirtoCommerce.CartModule.Core.Services;
using VirtoCommerce.CartModule.Data.Handlers;
using VirtoCommerce.CartModule.Data.Repositories;
using VirtoCommerce.CartModule.Data.Services;
using VirtoCommerce.CartModule.Web.JsonConverters;
using VirtoCommerce.Platform.Core.Bus;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Core.Security;

namespace VirtoCommerce.CartModule.Web
{
    public class Module : IModule
    {
        public ManifestModuleInfo ModuleInfo { get; set; }
        public void Initialize(IServiceCollection serviceCollection)
        {
            var configuration = serviceCollection.BuildServiceProvider().GetRequiredService<IConfiguration>();
            serviceCollection.AddTransient<ICartRepository, CartRepositoryImpl>();
            var connectionString = configuration.GetConnectionString("VirtoCommerce.Cart") ?? configuration.GetConnectionString("VirtoCommerce");
            serviceCollection.AddDbContext<CartDbContext>(options => options.UseSqlServer(connectionString));
            serviceCollection.AddSingleton<Func<ICartRepository>>(provider => () => provider.CreateScope().ServiceProvider.GetRequiredService<ICartRepository>());
            serviceCollection.AddSingleton<IShoppingCartService, ShoppingCartServiceImpl>();
            serviceCollection.AddSingleton<IShoppingCartSearchService, ShoppingCartSearchServiceImpl>();
            serviceCollection.AddSingleton<IShoppingCartTotalsCalculator, DefaultShoppingCartTotalsCalculator>();
            serviceCollection.AddSingleton<IShoppingCartBuilder, ShoppingCartBuilderImpl>();

            serviceCollection.AddSingleton<CartChangedEventHandler>();
            var providerSnapshot = serviceCollection.BuildServiceProvider();
            var inProcessBus = providerSnapshot.GetService<IHandlerRegistrar>();
            inProcessBus.RegisterHandler<CartChangedEvent>(async (message, token) => await providerSnapshot.GetService<CartChangedEventHandler>().Handle(message));
            inProcessBus.RegisterHandler<CartChangeEvent>(async (message, token) => await providerSnapshot.GetService<CartChangedEventHandler>().Handle(message));
        }

        public void PostInitialize(IApplicationBuilder appBuilder)
        {
            var permissionsProvider = appBuilder.ApplicationServices.GetRequiredService<IPermissionsRegistrar>();
            permissionsProvider.RegisterPermissions(ModuleConstants.Security.Permissions.AllPermissions.Select(x =>
                new Permission()
                {
                    GroupName = "Cart",
                    ModuleId = ModuleInfo.Id,
                    Name = x
                }).ToArray());

            var mvcJsonOptions = appBuilder.ApplicationServices.GetService<IOptions<MvcJsonOptions>>();
            mvcJsonOptions.Value.SerializerSettings.Converters.Add(new PolymorphicCartJsonConverter());

            using (var serviceScope = appBuilder.ApplicationServices.CreateScope())
            {
                using (var dbContext = serviceScope.ServiceProvider.GetRequiredService<CartDbContext>())
                {
                    dbContext.Database.EnsureCreated();
                    dbContext.Database.Migrate();
                }
            }
        }

        public void Uninstall()
        {
        }
    }
}
