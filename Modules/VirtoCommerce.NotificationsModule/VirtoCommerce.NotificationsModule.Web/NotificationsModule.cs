using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.NotificationsModule.Core.Abstractions;
using VirtoCommerce.NotificationsModule.Core.Services;
using VirtoCommerce.NotificationsModule.Data.Abstractions;
using VirtoCommerce.NotificationsModule.Data.Services;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.Platform.Data.Repositories;

namespace VirtoCommerce.NotificationsModule.Web
{
    public class NotificationsModule : IModule
    {
        public ManifestModuleInfo ModuleInfo { get; set; }

        public void Initialize(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<INotificationService, NotificationServiceImpl>();
            serviceCollection.AddTransient<INotificationTemplateService, NotificationTemplateServiceImpl>();
        }

        public void PostInitialize(IServiceProvider serviceProvider)
        {
            var settingsService = serviceProvider.GetRequiredService<ISettingsManager>();
            var platformRepository = serviceProvider.GetRequiredService<IPlatformRepository>();
        }

        public void Uninstall()
        {
        }
    }
}
