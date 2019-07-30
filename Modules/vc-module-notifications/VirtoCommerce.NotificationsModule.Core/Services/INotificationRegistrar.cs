using VirtoCommerce.NotificationsModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.NotificationsModule.Core.Services
{
    /// <summary>
    /// The registrar is for registration a type in the AbstractTypeFactory
    /// </summary>
    public interface INotificationRegistrar
    {
        void RegisterNotification<T>(params NotificationTemplate[] templates) where T : Notification;
        Notification GenerateNotification(string notificationType);
    }
}
