using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.NotificationsModule.Core.Abstractions;
using VirtoCommerce.NotificationsModule.Core.Model;
using VirtoCommerce.NotificationsModule.Data.Model;
using VirtoCommerce.NotificationsModule.Data.Repositories;
using VirtoCommerce.NotificationsModule.Data.Services;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Modularity;

namespace VirtoCommerce.NotificationsModule.Web
{
    public class NotificationsModule : IModule
    {
        public ManifestModuleInfo ModuleInfo { get; set; }

        public void Initialize(IServiceCollection serviceCollection)
        {
            var configuration = serviceCollection.BuildServiceProvider().GetService<IConfiguration>();
            serviceCollection.AddDbContext<NotificationDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("VirtoCommerce")));
            serviceCollection.AddTransient<INotificationRepository, NotificationRepositoryImpl>();
            serviceCollection.AddTransient<INotificationService, NotificationService>();
            serviceCollection.AddTransient<INotificationSearchService, NotificationSearchService>();
            serviceCollection.AddTransient<INotificationMessageService, NotificationMessageService>();

        }

        public void PostInitialize(IServiceProvider serviceProvider)
        {
            AbstractTypeFactory<Notification>.RegisterType<EmailNotification>().MapToType<NotificationEntity>();
            AbstractTypeFactory<Notification>.RegisterType<SmsNotification>().MapToType<NotificationEntity>();

            //Force migrations
            using (var serviceScope = serviceProvider.CreateScope())
            {
                var notificationDbContext = serviceScope.ServiceProvider.GetRequiredService<NotificationDbContext>();
                notificationDbContext.Database.Migrate();
                notificationDbContext.EnsureSeeded();
            }
        }

        public void Uninstall()
        {
        }
    }
}
