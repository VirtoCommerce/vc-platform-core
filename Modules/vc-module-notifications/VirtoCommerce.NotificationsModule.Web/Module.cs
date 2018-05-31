using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using VirtoCommerce.NotificationsModule.Core.Model;
using VirtoCommerce.NotificationsModule.Core.Services;
using VirtoCommerce.NotificationsModule.Data.Model;
using VirtoCommerce.NotificationsModule.Data.Repositories;
using VirtoCommerce.NotificationsModule.Data.Senders;
using VirtoCommerce.NotificationsModule.Data.Services;
using VirtoCommerce.NotificationsModule.LiquidRenderer;
using VirtoCommerce.NotificationsModule.SendGrid;
using VirtoCommerce.NotificationsModule.Smtp;
using VirtoCommerce.NotificationsModule.Web.Infrastructure;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Core.Notifications;

namespace VirtoCommerce.NotificationsModule.Web
{
    public class Module : IModule
    {
        public ManifestModuleInfo ModuleInfo { get; set; }

        public void Initialize(IServiceCollection serviceCollection)
        {
            var snapshot = serviceCollection.BuildServiceProvider();
            var configuration = snapshot.GetService<IConfiguration>();
            serviceCollection.AddDbContext<NotificationDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("VirtoCommerce")));
            serviceCollection.AddScoped<INotificationRepository, NotificationRepository>();
            serviceCollection.AddScoped<Func<INotificationRepository>>(provider => () => provider.CreateScope().ServiceProvider.GetService<INotificationRepository>());
            serviceCollection.AddScoped<INotificationService, NotificationService>();
            serviceCollection.AddScoped<INotificationRegistrar, NotificationService>();
            serviceCollection.AddScoped<INotificationSearchService, NotificationSearchService>();
            serviceCollection.AddScoped<INotificationMessageService, NotificationMessageService>();
            serviceCollection.AddTransient<INotificationSender, NotificationSender>();
            serviceCollection.AddTransient<INotificationTemplateRenderer, LiquidTemplateRenderer>();
            serviceCollection.AddSingleton<INotificationMessageSenderProviderFactory, NotificationMessageSenderProviderFactory>();
            serviceCollection.AddTransient<INotificationMessageSender, SmtpEmailNotificationMessageSender>();
            serviceCollection.AddTransient<INotificationMessageSender, SendGridEmailNotificationMessageSender>();
            serviceCollection.AddTransient<IEmailSender, EmailNotificationMessageSender>();

            serviceCollection.Configure<EmailSendingOptions>(configuration.GetSection("Notifications"));
            serviceCollection.Configure<SmtpSenderOptions>(configuration.GetSection("Notifications:Smtp"));
            serviceCollection.Configure<SendGridSenderOptions>(configuration.GetSection("Notifications:SendGrid"));
        }

        public void PostInitialize(IServiceProvider serviceProvider)
        {
            AbstractTypeFactory<Notification>.RegisterType<EmailNotification>().MapToType<NotificationEntity>();
            AbstractTypeFactory<Notification>.RegisterType<SmsNotification>().MapToType<NotificationEntity>();
            AbstractTypeFactory<NotificationTemplate>.RegisterType<EmailNotificationTemplate>().MapToType<NotificationTemplateEntity>();
            AbstractTypeFactory<NotificationTemplate>.RegisterType<SmsNotificationTemplate>().MapToType<NotificationTemplateEntity>();
            AbstractTypeFactory<NotificationMessage>.RegisterType<EmailNotificationMessage>().MapToType<NotificationMessageEntity>();
            AbstractTypeFactory<NotificationMessage>.RegisterType<SmsNotificationMessage>().MapToType<NotificationMessageEntity>();
            AbstractTypeFactory<NotificationEntity>.RegisterType<EmailNotificationEntity>();
            AbstractTypeFactory<NotificationEntity>.RegisterType<SmsNotificationEntity>();

            var mvcJsonOptions = serviceProvider.GetService<IOptions<MvcJsonOptions>>();
            mvcJsonOptions.Value.SerializerSettings.Converters.Add(new PolymorphicJsonConverter());

            using (var notificationDbContext = serviceProvider.GetRequiredService<NotificationDbContext>())
            {
                notificationDbContext.Database.EnsureCreated();
                notificationDbContext.Database.Migrate();
            }

            var configuration = serviceProvider.GetService<IConfiguration>();
            var notificationGateway = configuration.GetSection("Notifications:Gateway").Value;
            var notificationMessageSenderProviderFactory = serviceProvider.GetService<INotificationMessageSenderProviderFactory>();
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
    }
}
