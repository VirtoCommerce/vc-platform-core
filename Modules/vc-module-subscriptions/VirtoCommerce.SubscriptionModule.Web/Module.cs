using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using VirtoCommerce.NotificationsModule.Core.Services;
using VirtoCommerce.OrdersModule.Core.Events;
using VirtoCommerce.Platform.Core.Bus;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.ExportImport;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.SubscriptionModule.Core.Events;
using VirtoCommerce.SubscriptionModule.Core.ModuleConstants;
using VirtoCommerce.SubscriptionModule.Core.Services;
using VirtoCommerce.SubscriptionModule.Data.Handlers;
using VirtoCommerce.SubscriptionModule.Data.Notifications;
using VirtoCommerce.SubscriptionModule.Data.Repositories;
using VirtoCommerce.SubscriptionModule.Data.Services;
using VirtoCommerce.SubscriptionModule.Web.BackgroundJobs;
using VirtoCommerce.SubscriptionModule.Web.ExportImport;
using VirtoCommerce.SubscriptionModule.Web.JsonConverters;

namespace VirtoCommerce.SubscriptionModule.Web
{
    public class Module : IModule, IExportSupport, IImportSupport
    {
        private const string _connectionStringName = "VirtoCommerce";

        private IApplicationBuilder _applicationBuilder;
        

        #region IModule Members

        public ManifestModuleInfo ModuleInfo { get; set; }

        public void Initialize(IServiceCollection serviceCollection)
        {
            var serviceProvider = serviceCollection.BuildServiceProvider();

            //Registration welcome email notification.
            var eventHandlerRegistrar = serviceProvider.GetRequiredService<IHandlerRegistrar>();
            eventHandlerRegistrar.RegisterHandler<OrderChangedEvent>(async (message, token) => await serviceProvider.GetRequiredService<CreateSubscriptionOrderChangedEventHandler>().Handle(message));
            eventHandlerRegistrar.RegisterHandler<OrderChangedEvent>(async (message, token) => await serviceProvider.GetRequiredService<LogChangesSubscriptionChangedEventHandler>().Handle(message));
            eventHandlerRegistrar.RegisterHandler<SubscriptionChangedEvent>(async (message, token) => await serviceProvider.GetRequiredService<LogChangesSubscriptionChangedEventHandler>().Handle(message));
            eventHandlerRegistrar.RegisterHandler<SubscriptionChangedEvent>(async (message, token) => await serviceProvider.GetRequiredService<SendNotificationsSubscriptionChangedEventHandler>().Handle(message));

            var configuration = serviceProvider.GetRequiredService<IConfiguration>();
            serviceCollection.AddDbContext<SubscriptionDbContext>(options => options.UseSqlServer(configuration.GetConnectionString(_connectionStringName)));
            serviceCollection.AddTransient<ISubscriptionRepository, SubscriptionRepositoryImpl>();
            serviceCollection.AddSingleton<Func<ISubscriptionRepository>>(provider => () => provider.CreateScope().ServiceProvider.GetRequiredService<ISubscriptionRepository>());
         
            serviceCollection.AddTransient<ISubscriptionService, SubscriptionServiceImpl>();
            serviceCollection.AddTransient<ISubscriptionSearchService, SubscriptionServiceImpl>();
            serviceCollection.AddTransient<IPaymentPlanService, PaymentPlanService>();
            serviceCollection.AddTransient<IPaymentPlanSearchService, PaymentPlanService>();
            serviceCollection.AddTransient<ISubscriptionBuilder, SubscriptionBuilderImpl>();

            serviceCollection.AddSingleton<SubscriptionExportImport>();
        }

        public void PostInitialize(IApplicationBuilder applicationBuilder)
        {
            _applicationBuilder = applicationBuilder;

            // Register module permissions
            var permissionsRegistrar = applicationBuilder.ApplicationServices.GetRequiredService<IPermissionsRegistrar>();
            var permissions = ModulePermissions.AllPermissions.Select(permissionName => new Permission()
            {
                GroupName = "Subscription",
                Name = permissionName
            });
            permissionsRegistrar.RegisterPermissions(permissions.ToArray());

            //Register setting in the store level
            var settingsRegistrar = applicationBuilder.ApplicationServices.GetRequiredService<ISettingsRegistrar>();
            settingsRegistrar.RegisterSettings(ModuleSettings.AllSettings, ModuleInfo.Id);

            // TODO: how to simulate this?
            //var storeLevelSettings = new[] { "Subscription.EnableSubscriptions" };
            //settingsManager.RegisterModuleSettings("VirtoCommerce.Store", settingsManager.GetModuleSettings(ModuleInfo.Id).Where(x => storeLevelSettings.Contains(x.Name)).ToArray());

            //Schedule periodic subscription processing job
            var settingsManager = applicationBuilder.ApplicationServices.GetRequiredService<ISettingsManager>();
            var processJobEnable = settingsManager.GetValue(ModuleSettings.EnableSubscriptionProcessJob.Name, true);
            if (processJobEnable)
            {
                var cronExpression = settingsManager.GetValue(ModuleSettings.CronExpression.Name, "0/5 * * * *");
                RecurringJob.AddOrUpdate<ProcessSubscriptionJob>("ProcessSubscriptionJob", x => x.Process(), cronExpression);
            }
            else
            {
                RecurringJob.RemoveIfExists("ProcessSubscriptionJob");
            }

            var createOrderJobEnable = settingsManager.GetValue(ModuleSettings.EnableSubscriptionOrdersCreateJob.Name, true);
            if (createOrderJobEnable)
            {
                var cronExpressionOrder = settingsManager.GetValue(ModuleSettings.CronExpressionOrdersJob.Name, "0/15 * * * *");
                RecurringJob.AddOrUpdate<CreateRecurrentOrdersJob>("ProcessSubscriptionOrdersJob", x => x.Process(), cronExpressionOrder);
            }
            else
            {
                RecurringJob.RemoveIfExists("ProcessSubscriptionOrdersJob");
            }

            var notificationRegistrar = applicationBuilder.ApplicationServices.GetService<INotificationRegistrar>();
            notificationRegistrar.RegisterNotification<NewSubscriptionEmailNotification>();
            notificationRegistrar.RegisterNotification<SubscriptionCanceledEmailNotification>();

            //Next lines allow to use polymorph types in API controller methods
            var mvcJsonOptions = applicationBuilder.ApplicationServices.GetService<IOptions<MvcJsonOptions>>();
            mvcJsonOptions.Value.SerializerSettings.Converters.Add(new PolymorphicSubscriptionJsonConverter());

            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var subscriptionDbContext = serviceScope.ServiceProvider.GetRequiredService<SubscriptionDbContext>();
                subscriptionDbContext.Database.EnsureCreated();
                subscriptionDbContext.Database.Migrate();
            }
        }

        public void Uninstall()
        {
        }

        #endregion


        public async Task ExportAsync(Stream outStream, ExportImportOptions options, Action<ExportImportProgressInfo> progressCallback,
            ICancellationToken cancellationToken)
        {
            await _applicationBuilder.ApplicationServices.GetRequiredService<SubscriptionExportImport>().DoExportAsync(outStream,
                progressCallback, cancellationToken);
        }

        public async Task ImportAsync(Stream inputStream, ExportImportOptions options, Action<ExportImportProgressInfo> progressCallback,
            ICancellationToken cancellationToken)
        {
            await _applicationBuilder.ApplicationServices.GetRequiredService<SubscriptionExportImport>().DoImportAsync(inputStream,
                progressCallback, cancellationToken);
        }
    }
}
