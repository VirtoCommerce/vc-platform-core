using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.LicensingModule.Core;
using VirtoCommerce.LicensingModule.Core.Events;
using VirtoCommerce.LicensingModule.Core.Model;
using VirtoCommerce.LicensingModule.Core.Services;
using VirtoCommerce.LicensingModule.Data.Handlers;
using VirtoCommerce.LicensingModule.Data.Repositories;
using VirtoCommerce.LicensingModule.Data.Services;
using VirtoCommerce.Platform.Core.Bus;
using VirtoCommerce.Platform.Core.ChangeLog;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.Platform.Data.Extensions;

namespace VirtoCommerce.LicensingModule.Web
{
    public class Module : IModule
    {
        public ManifestModuleInfo ModuleInfo { get; set; }
        public void Initialize(IServiceCollection serviceCollection)
        {
            var snapshot = serviceCollection.BuildServiceProvider();
            var configuration = snapshot.GetService<IConfiguration>();
            serviceCollection.AddTransient<ILicenseRepository, LicenseRepository>();
            serviceCollection.AddDbContext<LicenseDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("VirtoCommerce")));
            serviceCollection.AddTransient<Func<ILicenseRepository>>(provider => () => provider.CreateScope().ServiceProvider.GetService<ILicenseRepository>());
            serviceCollection.AddTransient(provider => new LogLicenseChangedEventHandler(provider.CreateScope().ServiceProvider.GetService<IChangeLogService>()));
            var providerSnapshot = serviceCollection.BuildServiceProvider();
            var inProcessBus = providerSnapshot.GetService<IHandlerRegistrar>();
            inProcessBus.RegisterHandler<LicenseChangedEvent>(async (message, token) => await providerSnapshot.GetService<LogLicenseChangedEventHandler>().Handle(message));
            inProcessBus.RegisterHandler<LicenseSignedEvent>(async (message, token) => await providerSnapshot.GetService<LogLicenseChangedEventHandler>().Handle(message));
            serviceCollection.AddTransient<ILicenseService, LicenseService>();
            serviceCollection.AddOptions<LicenseOptions>().Bind(configuration.GetSection("VirtoCommerce")).ValidateDataAnnotations();
        }

        public void PostInitialize(IApplicationBuilder applicationBuilder)
        {
            var settingsRegistrar = applicationBuilder.ApplicationServices.GetRequiredService<ISettingsRegistrar>();
            settingsRegistrar.RegisterSettings(ModuleConstants.Settings.AllSettings, ModuleInfo.Id);

            var permissionsProvider = applicationBuilder.ApplicationServices.GetRequiredService<IPermissionsRegistrar>();
            permissionsProvider.RegisterPermissions(ModuleConstants.Security.Permissions.AllPermissions.Select(x =>
                new Permission()
                {
                    GroupName = "Licensing",
                    Name = x,
                    ModuleId = ModuleInfo.Id
                }).ToArray());

            //Force migrations
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var licenseDbContext = serviceScope.ServiceProvider.GetRequiredService<LicenseDbContext>();
                licenseDbContext.Database.MigrateIfNotApplied(MigrationName.GetUpdateV2MigrationName(ModuleInfo.Id));
                licenseDbContext.Database.EnsureCreated();
                licenseDbContext.Database.Migrate();
            }
        }

        public void Uninstall()
        {
        }
    }
}
