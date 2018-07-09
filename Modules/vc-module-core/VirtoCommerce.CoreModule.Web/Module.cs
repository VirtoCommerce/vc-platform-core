using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.CoreModule.Core.Registrars;
using VirtoCommerce.CoreModule.Core.Services;
using VirtoCommerce.CoreModule.Data.Registrars;
using VirtoCommerce.CoreModule.Data.Repositories;
using VirtoCommerce.CoreModule.Data.Services;
using VirtoCommerce.Platform.Core.Modularity;

namespace VirtoCommerce.CoreModule.Web
{
    public class Module : IModule
    {
        public ManifestModuleInfo ModuleInfo { get; set; }

        public void Initialize(IServiceCollection serviceCollection)
        {
            var configuration = serviceCollection.BuildServiceProvider().GetRequiredService<IConfiguration>();
            serviceCollection.AddTransient<ICoreRepository, CoreRepositoryImpl>();
            var connectionString = configuration.GetConnectionString("VirtoCommerce.Core") ?? configuration.GetConnectionString("VirtoCommerce");
            serviceCollection.AddDbContext<CoreDbContext>(options => options.UseSqlServer(connectionString));
            serviceCollection.AddSingleton<Func<ICoreRepository>>(provider => () => provider.CreateScope().ServiceProvider.GetRequiredService<ICoreRepository>());
            serviceCollection.AddSingleton<ISeoService, SeoServiceImpl>();
            serviceCollection.AddSingleton<ICurrencyService, CurrencyServiceImpl>();
            serviceCollection.AddSingleton<IPackageTypesService, PackageTypesServiceImpl>();
            serviceCollection.AddSingleton<IShippingMethodsRegistrar>(new ShippingMethodsRegistrarImpl());
            serviceCollection.AddSingleton<IPaymentMethodsRegistrar>(new PaymentMethodsRegistrarImpl());
            serviceCollection.AddSingleton<ITaxRegistrar>(new TaxRegistrarImpl());
        }

        public void PostInitialize(IApplicationBuilder appBuilder)
        {

            using (var serviceScope = appBuilder.ApplicationServices.CreateScope())
            {
                var dbContext = serviceScope.ServiceProvider.GetRequiredService<CoreDbContext>();
                dbContext.Database.EnsureCreated();
                dbContext.Database.Migrate();
            }
        }

        public void Uninstall()
        {
        }
    }
}

