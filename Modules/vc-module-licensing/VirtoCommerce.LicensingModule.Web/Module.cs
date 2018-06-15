using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.LicensingModule.Core.Events;
using VirtoCommerce.LicensingModule.Core.Services;
using VirtoCommerce.LicensingModule.Data.Observers;
using VirtoCommerce.LicensingModule.Data.Repositories;
using VirtoCommerce.LicensingModule.Data.Services;
using VirtoCommerce.Platform.Core.Modularity;

namespace VirtoCommerce.LicensingModule.Web
{
    public class Module : IModule
    {
        public ManifestModuleInfo ModuleInfo { get; set; }
        public void Initialize(IServiceCollection serviceCollection)
        {
            var snapshot = serviceCollection.BuildServiceProvider();
            var configuration = snapshot.GetService<IConfiguration>();
            serviceCollection.AddDbContext<LicenseDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("VirtoCommerce")));
            serviceCollection.AddScoped<ILicenseRepository, LicenseRepository>();
            serviceCollection.AddScoped<Func<ILicenseRepository>>(provider => () => provider.CreateScope().ServiceProvider.GetService<ILicenseRepository>());
            //log License activations and changes
            serviceCollection.AddSingleton<IObserver<LicenseSignedEvent>, LogLicenseEventsObserver>();
            serviceCollection.AddSingleton<IObserver<LicenseChangedEvent>, LogLicenseEventsObserver>();
            serviceCollection.AddSingleton<ILicenseService, LicenseService>();
        }

        public void PostInitialize(IServiceProvider serviceProvider)
        {
        }

        public void Uninstall()
        {
        }
    }
}
