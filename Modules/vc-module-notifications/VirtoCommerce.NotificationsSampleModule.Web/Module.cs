using System;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.NotificationsModule.Core.Model;
using VirtoCommerce.NotificationsModule.Core.Services;
using VirtoCommerce.NotificationsModule.Data.Model;
using VirtoCommerce.NotificationsSampleModule.Web.Models;
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
            
        }

        public void PostInitialize(IServiceProvider serviceProvider)
        {
            AbstractTypeFactory<Notification>.RegisterType<TwitterNotification>().MapToType<NotificationEntity>();
            AbstractTypeFactory<NotificationTemplate>.RegisterType<TwitterNotificationTemplate>().MapToType<NotificationTemplateEntity>();
            AbstractTypeFactory<NotificationMessage>.RegisterType<TwitterNotificationMessage>().MapToType<NotificationMessageEntity>();
            AbstractTypeFactory<NotificationEntity>.RegisterType<TwitterNotificationEntity>();
            var registrar = serviceProvider.GetService<INotificationRegistrar>();
            registrar.RegisterNotification<PostTwitterNotification>();
            registrar.RegisterNotification<RegistrationEmailNotification>();
        }

        public void Uninstall()
        {
        }
    }
}
