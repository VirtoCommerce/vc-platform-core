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
using VirtoCommerce.NotificationsModule.Core.Services;
using VirtoCommerce.OrdersModule.Core;
using VirtoCommerce.OrdersModule.Core.Events;
using VirtoCommerce.OrdersModule.Core.Notifications;
using VirtoCommerce.OrdersModule.Core.Services;
using VirtoCommerce.OrdersModule.Data.ExportImport;
using VirtoCommerce.OrdersModule.Data.Handlers;
using VirtoCommerce.OrdersModule.Data.Repositories;
using VirtoCommerce.OrdersModule.Data.Services;
using VirtoCommerce.OrdersModule.Web.JsonConverters;
using VirtoCommerce.Platform.Core.Bus;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.ExportImport;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.Platform.Core.Settings;

namespace VirtoCommerce.OrdersModule.Web
{
    public class Module : IModule, IExportSupport, IImportSupport
    {
        public ManifestModuleInfo ModuleInfo { get; set; }
        private IApplicationBuilder _appBuilder;
        public void Initialize(IServiceCollection serviceCollection)
        {
            var configuration = serviceCollection.BuildServiceProvider().GetRequiredService<IConfiguration>();
            serviceCollection.AddTransient<IOrderRepository, OrderRepositoryImpl>();
            var connectionString = configuration.GetConnectionString("VirtoCommerce.Orders") ?? configuration.GetConnectionString("VirtoCommerce");
            serviceCollection.AddDbContext<OrderDbContext>(options => options.UseSqlServer(connectionString));
            serviceCollection.AddSingleton<Func<IOrderRepository>>(provider => () => provider.CreateScope().ServiceProvider.GetRequiredService<IOrderRepository>());
            serviceCollection.AddSingleton<ICustomerOrderSearchService, CustomerOrderSearchServiceImpl>();
            serviceCollection.AddSingleton<ICustomerOrderService, CustomerOrderServiceImpl>();
            serviceCollection.AddSingleton<ICustomerOrderBuilder, CustomerOrderBuilderImpl>();
            serviceCollection.AddSingleton<ICustomerOrderTotalsCalculator, DefaultCustomerOrderTotalsCalculator>();
            serviceCollection.AddSingleton<OrderExportImport>();
            serviceCollection.AddSingleton<OrderChangedEvent>();
            serviceCollection.AddSingleton<AdjustInventoryOrderChangedEventHandler>();
            serviceCollection.AddSingleton<CancelPaymentOrderChangedEventHandler>();
            serviceCollection.AddSingleton<LogChangesOrderChangedEventHandler>();
            serviceCollection.AddSingleton<SendNotificationsOrderChangedEventHandler>();
            serviceCollection.AddSingleton<PolymorphicOperationJsonConverter>();
        }

        public void PostInitialize(IApplicationBuilder appBuilder)
        {
            _appBuilder = appBuilder;

            var settingsRegistrar = appBuilder.ApplicationServices.GetRequiredService<ISettingsRegistrar>();
            settingsRegistrar.RegisterSettings(ModuleConstants.Settings.General.AllSettings, ModuleInfo.Id);

            var permissionsProvider = appBuilder.ApplicationServices.GetRequiredService<IPermissionsRegistrar>();
            permissionsProvider.RegisterPermissions(ModuleConstants.Security.Permissions.AllPermissions.Select(x =>
                new Permission()
                {
                    GroupName = "Orders",
                    ModuleId = ModuleInfo.Id,
                    Name = x
                }).ToArray());

            var inProcessBus = appBuilder.ApplicationServices.GetService<IHandlerRegistrar>();
            inProcessBus.RegisterHandler<OrderChangedEvent>(async (message, token) => await appBuilder.ApplicationServices.GetService<AdjustInventoryOrderChangedEventHandler>().Handle(message));
            inProcessBus.RegisterHandler<OrderChangedEvent>(async (message, token) => await appBuilder.ApplicationServices.GetService<CancelPaymentOrderChangedEventHandler>().Handle(message));
            inProcessBus.RegisterHandler<OrderChangedEvent>(async (message, token) => await appBuilder.ApplicationServices.GetService<LogChangesOrderChangedEventHandler>().Handle(message));
            inProcessBus.RegisterHandler<OrderChangedEvent>(async (message, token) => await appBuilder.ApplicationServices.GetService<SendNotificationsOrderChangedEventHandler>().Handle(message));

            using (var serviceScope = appBuilder.ApplicationServices.CreateScope())
            {
                var dbContext = serviceScope.ServiceProvider.GetRequiredService<OrderDbContext>();
                dbContext.Database.EnsureCreated();
                dbContext.Database.Migrate();
            }

            var notificationRegistrar = appBuilder.ApplicationServices.GetService<INotificationRegistrar>();
            notificationRegistrar.RegisterNotification<CancelOrderEmailNotification>();
            notificationRegistrar.RegisterNotification<InvoiceEmailNotification>();
            notificationRegistrar.RegisterNotification<NewOrderStatusEmailNotification>();
            notificationRegistrar.RegisterNotification<OrderCreateEmailNotification>();
            notificationRegistrar.RegisterNotification<OrderPaidEmailNotification>();
            notificationRegistrar.RegisterNotification<OrderSentEmailNotification>();

            // enable polymorphic types in API controller methods
            var mvcJsonOptions = appBuilder.ApplicationServices.GetService<IOptions<MvcJsonOptions>>();
            mvcJsonOptions.Value.SerializerSettings.Converters.Add(appBuilder.ApplicationServices.GetService<PolymorphicOperationJsonConverter>());
        }

        public void Uninstall()
        {
        }

        public Task ExportAsync(Stream outStream, ExportImportOptions options, Action<ExportImportProgressInfo> progressCallback,
            ICancellationToken cancellationToken)
        {
            return _appBuilder.ApplicationServices.GetRequiredService<OrderExportImport>().ExportAsync(outStream, options, progressCallback, cancellationToken);
        }

        public Task ImportAsync(Stream inputStream, ExportImportOptions options, Action<ExportImportProgressInfo> progressCallback,
            ICancellationToken cancellationToken)
        {
            return _appBuilder.ApplicationServices.GetRequiredService<OrderExportImport>().ImportAsync(inputStream, options, progressCallback, cancellationToken);
        }
    }
}
