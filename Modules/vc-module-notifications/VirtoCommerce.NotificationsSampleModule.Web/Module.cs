using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.NotificationsModule.Core.Services;
using VirtoCommerce.NotificationsModule.Data.Model;
using VirtoCommerce.NotificationsModule.Data.Repositories;
using VirtoCommerce.NotificationsSampleModule.Web.Models;
using VirtoCommerce.NotificationsSampleModule.Web.Repositories;
using VirtoCommerce.NotificationsSampleModule.Web.Types;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Modularity;

namespace VirtoCommerce.NotificationsSampleModule.Web
{
    public class Module : IModule
    {
        public ManifestModuleInfo ModuleInfo { get; set; }

        public void Initialize(IServiceCollection serviceCollection)
        {
            var snapshot = serviceCollection.BuildServiceProvider();
            var configuration = snapshot.GetService<IConfiguration>();
            serviceCollection.AddDbContext<TwitterNotificationDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("VirtoCommerce")));
            serviceCollection.AddTransient<INotificationRepository, TwitterNotificationRepository>();
            serviceCollection.AddSingleton<Func<INotificationRepository>>(provider => () => provider.CreateScope().ServiceProvider.GetService<INotificationRepository>());
        }

        public void PostInitialize(IApplicationBuilder appBuilder)
        {
            AbstractTypeFactory<NotificationEntity>.RegisterType<TwitterNotificationEntity>();
            var registrar = appBuilder.ApplicationServices.GetService<INotificationRegistrar>();
            registrar.RegisterNotification<PostTwitterNotification>();
            registrar.RegisterNotification<RegistrationEmailNotification>();

            using (var serviceScope = appBuilder.ApplicationServices.CreateScope())
            {
                using (var notificationDbContext = serviceScope.ServiceProvider.GetRequiredService<TwitterNotificationDbContext>())
                {
                    notificationDbContext.Database.EnsureCreated();
                    notificationDbContext.Database.Migrate();
                }
            }
        }

        public void Uninstall()
        {
        }
    }
}
