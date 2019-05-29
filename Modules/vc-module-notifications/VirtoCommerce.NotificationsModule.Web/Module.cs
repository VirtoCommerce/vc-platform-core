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
using VirtoCommerce.NotificationsModule.Core;
using VirtoCommerce.NotificationsModule.Core.Model;
using VirtoCommerce.NotificationsModule.Core.Services;
using VirtoCommerce.NotificationsModule.Data.ExportImport;
using VirtoCommerce.NotificationsModule.Data.JsonConverters;
using VirtoCommerce.NotificationsModule.Data.Model;
using VirtoCommerce.NotificationsModule.Data.Repositories;
using VirtoCommerce.NotificationsModule.Data.Senders;
using VirtoCommerce.NotificationsModule.Data.Services;
using VirtoCommerce.NotificationsModule.LiquidRenderer;
using VirtoCommerce.NotificationsModule.SendGrid;
using VirtoCommerce.NotificationsModule.Smtp;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.ExportImport;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Core.Notifications;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.Platform.Core.Settings;

namespace VirtoCommerce.NotificationsModule.Web
{
    public class Module : IModule, IExportSupport, IImportSupport
    {
        public ManifestModuleInfo ModuleInfo { get; set; }
        private IApplicationBuilder _appBuilder;

        public void Initialize(IServiceCollection serviceCollection)
        {
            var snapshot = serviceCollection.BuildServiceProvider();
            var configuration = snapshot.GetService<IConfiguration>();
            serviceCollection.AddDbContext<NotificationDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("VirtoCommerce")));
            serviceCollection.AddTransient<INotificationRepository, NotificationRepository>();
            serviceCollection.AddSingleton<Func<INotificationRepository>>(provider => () => provider.CreateScope().ServiceProvider.GetService<INotificationRepository>());
            serviceCollection.AddSingleton<INotificationService, NotificationService>();
            serviceCollection.AddSingleton<INotificationSearchService, NotificationSearchService>();
            serviceCollection.AddSingleton<INotificationRegistrar, NotificationService>();
            serviceCollection.AddSingleton<INotificationMessageService, NotificationMessageService>();
            serviceCollection.AddSingleton<INotificationMessageSearchService, NotificationMessageSearchService>();
            serviceCollection.AddSingleton<INotificationSender, NotificationSender>();
            serviceCollection.AddSingleton<INotificationTemplateRenderer, LiquidTemplateRenderer>();
            serviceCollection.AddSingleton<INotificationMessageSenderProviderFactory, NotificationMessageSenderProviderFactory>();
            serviceCollection.AddTransient<INotificationMessageSender, SmtpEmailNotificationMessageSender>();
            serviceCollection.AddTransient<INotificationMessageSender, SendGridEmailNotificationMessageSender>();
            serviceCollection.AddTransient<IEmailSender, EmailNotificationMessageSender>();
            serviceCollection.AddSingleton<NotificationsExportImport>();
            serviceCollection.Configure<EmailSendingOptions>(configuration.GetSection("Notifications"));
            serviceCollection.Configure<SmtpSenderOptions>(configuration.GetSection("Notifications:Smtp"));
            serviceCollection.Configure<SendGridSenderOptions>(configuration.GetSection("Notifications:SendGrid"));
        }

        public void PostInitialize(IApplicationBuilder appBuilder)
        {
            _appBuilder = appBuilder;

            AbstractTypeFactory<NotificationTemplate>.RegisterType<EmailNotificationTemplate>();
            AbstractTypeFactory<NotificationTemplate>.RegisterType<SmsNotificationTemplate>();

            AbstractTypeFactory<NotificationMessage>.RegisterType<EmailNotificationMessage>();
            AbstractTypeFactory<NotificationMessage>.RegisterType<SmsNotificationMessage>();

            AbstractTypeFactory<NotificationEntity>.RegisterType<EmailNotificationEntity>();
            AbstractTypeFactory<NotificationEntity>.RegisterType<SmsNotificationEntity>();

            AbstractTypeFactory<NotificationTemplateEntity>.RegisterType<EmailNotificationTemplateEntity>();
            AbstractTypeFactory<NotificationTemplateEntity>.RegisterType<SmsNotificationTemplateEntity>();

            AbstractTypeFactory<NotificationMessageEntity>.RegisterType<EmailNotificationMessageEntity>();
            AbstractTypeFactory<NotificationMessageEntity>.RegisterType<SmsNotificationMessageEntity>();


            var settingsRegistrar = appBuilder.ApplicationServices.GetRequiredService<ISettingsRegistrar>();
            settingsRegistrar.RegisterSettings(ModuleConstants.Settings.AllSettings, ModuleInfo.Id);

            var permissionsProvider = appBuilder.ApplicationServices.GetRequiredService<IPermissionsRegistrar>();
            permissionsProvider.RegisterPermissions(ModuleConstants.Security.Permissions.AllPermissions.Select(x =>
                new Permission()
                {
                    GroupName = "Notifications",
                    ModuleId = ModuleInfo.Id,
                    Name = x
                }).ToArray());

            var mvcJsonOptions = appBuilder.ApplicationServices.GetService<IOptions<MvcJsonOptions>>();
            mvcJsonOptions.Value.SerializerSettings.Converters.Add(new NotificationPolymorphicJsonConverter());

            using (var serviceScope = appBuilder.ApplicationServices.CreateScope())
            {
                using (var notificationDbContext = serviceScope.ServiceProvider.GetRequiredService<NotificationDbContext>())
                {
                    notificationDbContext.Database.EnsureCreated();
                    notificationDbContext.Database.Migrate();
                }
            }

            //TODO move out from here to projects
            var configuration = appBuilder.ApplicationServices.GetService<IConfiguration>();
            var notificationGateway = configuration.GetSection("Notifications:Gateway").Value;
            var notificationMessageSenderProviderFactory = appBuilder.ApplicationServices.GetService<INotificationMessageSenderProviderFactory>();
            switch (notificationGateway)
            {
                case "SendGrid":
                    notificationMessageSenderProviderFactory.RegisterSenderForType<EmailNotification, SendGridEmailNotificationMessageSender>();
                    break;
                default:
                    notificationMessageSenderProviderFactory.RegisterSenderForType<EmailNotification, SmtpEmailNotificationMessageSender>();
                    break;
            }

        }

        public void Uninstall()
        {
        }

        public async Task ExportAsync(Stream outStream, ExportImportOptions options, Action<ExportImportProgressInfo> progressCallback, ICancellationToken cancellationToken)
        {
            await _appBuilder.ApplicationServices.GetRequiredService<NotificationsExportImport>().DoExportAsync(outStream, progressCallback, cancellationToken);
        }

        public async Task ImportAsync(Stream inputStream, ExportImportOptions options, Action<ExportImportProgressInfo> progressCallback, ICancellationToken cancellationToken)
        {
            await _appBuilder.ApplicationServices.GetRequiredService<NotificationsExportImport>().DoImportAsync(inputStream, progressCallback, cancellationToken);
        }
    }
}
