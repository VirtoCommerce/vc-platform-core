using System.Linq;
using VirtoCommerce.NotificationsModule.Core.Model;
using VirtoCommerce.NotificationsModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.NotificationsModule.Data.Services
{
    public class NotificationRegistrar : INotificationRegistrar
    {
        public NotificationBuilder RegisterNotification<T>() where T : Notification
        {
            var result = new NotificationBuilder();
            AbstractTypeFactory<Notification>.RegisterType<T>().WithSetupAction((notification) =>
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
