using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using VirtoCommerce.NotificationsModule.Core.Abstractions;
using VirtoCommerce.NotificationsModule.Core.Model;
using VirtoCommerce.NotificationsModule.Data.Model;
using VirtoCommerce.NotificationsModule.Data.Repositories;
using VirtoCommerce.NotificationsModule.Data.Senders;
using VirtoCommerce.NotificationsModule.Data.Services;
using VirtoCommerce.NotificationsModule.LiguidRenderer;
using VirtoCommerce.NotificationsModule.Smtp;
using VirtoCommerce.NotificationsModule.Web.Infrastructure;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Modularity;

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
            serviceCollection.AddTransient<INotificationRepository, NotificationRepositoryImpl>();
            serviceCollection.AddSingleton<Func<INotificationRepository>>(provider => () => provider.CreateScope().ServiceProvider.GetService<INotificationRepository>());
            serviceCollection.AddScoped<INotificationService, NotificationService>();
            serviceCollection.AddScoped<INotificationRegistrar, NotificationService>();
            serviceCollection.AddScoped<INotificationSearchService, NotificationSearchService>();
            serviceCollection.AddScoped<INotificationMessageService, NotificationMessageService>();
            serviceCollection.AddTransient<INotificationSender, NotificationSender>();
            serviceCollection.AddTransient<INotificationTemplateRender, LiquidTemplateRenderer>();
            serviceCollection.AddTransient<INotificationMessageSender, SmtpEmailNotificationMessageSender>();
        }

        public void PostInitialize(IServiceProvider serviceProvider)
        {
            AbstractTypeFactory<Notification>.RegisterType<EmailNotification>().MapToType<NotificationEntity>();
            AbstractTypeFactory<Notification>.RegisterType<SmsNotification>().MapToType<NotificationEntity>();
            AbstractTypeFactory<NotificationTemplate>.RegisterType<EmailNotificationTemplate>().MapToType<NotificationTemplateEntity>();
            AbstractTypeFactory<NotificationTemplate>.RegisterType<SmsNotificationTemplate>().MapToType<NotificationTemplateEntity>();
            AbstractTypeFactory<NotificationMessage>.RegisterType<EmailNotificationMessage>().MapToType<NotificationMessageEntity>();
            AbstractTypeFactory<NotificationMessage>.RegisterType<SmsNotificationMessage>().MapToType<NotificationMessageEntity>();

            var mvcJsonOptions = serviceProvider.GetService<IOptions<MvcJsonOptions>>();
            mvcJsonOptions.Value.SerializerSettings.Converters.Add(new PolymorphicJsonConverter());

            //Force migrations
            using (var serviceScope = serviceProvider.CreateScope())
            {
                var notificationDbContext = serviceScope.ServiceProvider.GetRequiredService<NotificationDbContext>();
                notificationDbContext.Database.Migrate();
            }

            //todo for tests
            var registrar = serviceProvider.GetService<INotificationRegistrar>();
            registrar.RegisterNotification<RegistrationEmailNotification>();
        }

        public void Uninstall()
        {
        }
    }

    public class RegistrationEmailNotification : EmailNotification
    {
        public CustomerOrder Context { get; set; }
    }

    public class CustomerOrder
    {
        public string Number { get; set; }
    }
}
