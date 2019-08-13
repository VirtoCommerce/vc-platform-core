using System;
using System.Linq;
using VirtoCommerce.NotificationsModule.Core.Model;
using VirtoCommerce.NotificationsModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.NotificationsModule.Data.Services
{
    public class NotificationRegistrar : INotificationRegistrar
    {
        public NotificationBuilder RegisterNotification<T>(Func<Notification> factory = null) where T : Notification
        {
            var result = new NotificationBuilder();

            if (AbstractTypeFactory<Notification>.AllTypeInfos.All(t => t.Type != typeof(T)))
            {
                AbstractTypeFactory<Notification>.RegisterType<T>().WithFactory(factory).WithSetupAction((notification) =>
                {
                    if (result.PredefinedTemplates != null)
                    {
                        notification.Templates = result.PredefinedTemplates.ToList();
                    }
                });
            }
                
            return result;
        }

        public NotificationBuilder OverrideNotificationType<OldType,NewType>(Func<Notification> factory = null) where OldType : Notification where NewType : Notification
        {
            var result = new NotificationBuilder();

            AbstractTypeFactory<Notification>.OverrideType<OldType, NewType>().WithFactory(factory).WithSetupAction((notification) =>
            {
                if (result.PredefinedTemplates != null)
                {
                    notification.Templates = result.PredefinedTemplates.ToList();
                }
            });

            return result;
        }
    }
}
