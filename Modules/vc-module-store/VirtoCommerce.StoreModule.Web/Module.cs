using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using VirtoCommerce.CoreModule.Core.Services;
using VirtoCommerce.NotificationsModule.Core.Services;
using VirtoCommerce.Platform.Core.Bus;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.ExportImport;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.StoreModule.Core;
using VirtoCommerce.StoreModule.Core.Events;
using VirtoCommerce.StoreModule.Core.Notifications;
using VirtoCommerce.StoreModule.Core.Services;
using VirtoCommerce.StoreModule.Data.ExportImport;
using VirtoCommerce.StoreModule.Data.Handlers;
using VirtoCommerce.StoreModule.Data.Repositories;
using VirtoCommerce.StoreModule.Data.Services;
using VirtoCommerce.StoreModule.Web.JsonConverters;

namespace VirtoCommerce.StoreModule.Web
{
    public class Module : IModule, IExportSupport, IImportSupport
    {
        public ManifestModuleInfo ModuleInfo { get; set; }
        private IApplicationBuilder _appBuilder;
        public void Initialize(IServiceCollection serviceCollection)
        {
            var snapshot = serviceCollection.BuildServiceProvider();
            var configuration = snapshot.GetService<IConfiguration>();
            var connectionString = configuration.GetConnectionString("VirtoCommerce.Store") ?? configuration.GetConnectionString("VirtoCommerce");
            serviceCollection.AddDbContext<StoreDbContext>(options => options.UseSqlServer(connectionString));
            serviceCollection.AddTransient<IStoreRepository, StoreRepositoryImpl>();
            serviceCollection.AddSingleton<Func<IStoreRepository>>(provider => () => provider.CreateScope().ServiceProvider.GetService<IStoreRepository>());
            serviceCollection.AddSingleton<IStoreService, StoreServiceImpl>();
            serviceCollection.AddSingleton<IStoreSearchService, StoreSearchServiceImpl>();
            serviceCollection.AddSingleton<StoreExportImport>();
            serviceCollection.AddSingleton<StoreChangedEventHandler>();


        }

        public void PostInitialize(IApplicationBuilder appBuilder)
        {
            _appBuilder = appBuilder;

            var settingsRegistrar = appBuilder.ApplicationServices.GetRequiredService<ISettingsRegistrar>();
            settingsRegistrar.RegisterSettings(ModuleConstants.Settings.AllSettings, ModuleInfo.Id);

            var permissionsProvider = appBuilder.ApplicationServices.GetRequiredService<IPermissionsRegistrar>();
            permissionsProvider.RegisterPermissions(ModuleConstants.Security.Permissions.AllPermissions.Select(x =>
                new Permission()
                {
                    GroupName = "Store",
                    ModuleId = ModuleInfo.Id,
                    Name = x
                }).ToArray());

            using (var serviceScope = appBuilder.ApplicationServices.CreateScope())
            {
                var dbContext = serviceScope.ServiceProvider.GetRequiredService<StoreDbContext>();
                dbContext.Database.EnsureCreated();
                dbContext.Database.Migrate();
            }

            var mvcJsonOptions = appBuilder.ApplicationServices.GetService<IOptions<MvcJsonOptions>>();
            var paymentMethodsRegistrar = appBuilder.ApplicationServices.GetService<IPaymentMethodsRegistrar>();
            var shippingMethodsRegistrar = appBuilder.ApplicationServices.GetService<IShippingMethodsRegistrar>();
            var taxRegistrar = appBuilder.ApplicationServices.GetService<ITaxRegistrar>();
            mvcJsonOptions.Value.SerializerSettings.Converters.Add(new PolymorphicStoreJsonConverter(paymentMethodsRegistrar, shippingMethodsRegistrar, taxRegistrar));

            var inProcessBus = appBuilder.ApplicationServices.GetService<IHandlerRegistrar>();
            inProcessBus.RegisterHandler<StoreChangedEvent>(async (message, token) => await appBuilder.ApplicationServices.GetService<StoreChangedEventHandler>().Handle(message));

            var registrar = appBuilder.ApplicationServices.GetService<INotificationRegistrar>();
            registrar.RegisterNotification<StoreDynamicEmailNotification>();
        }

        public void Uninstall()
        {
        }

        public async Task ExportAsync(Stream outStream, ExportImportOptions options, Action<ExportImportProgressInfo> progressCallback, ICancellationToken cancellationToken)
        {
            await _appBuilder.ApplicationServices.GetRequiredService<StoreExportImport>().ExportAsync(outStream, options, progressCallback, cancellationToken);
        }

        public async Task ImportAsync(Stream inputStream, ExportImportOptions options, Action<ExportImportProgressInfo> progressCallback, ICancellationToken cancellationToken)
        {
            await _appBuilder.ApplicationServices.GetRequiredService<StoreExportImport>().ImportAsync(inputStream, options, progressCallback, cancellationToken);
        }
    }
}
