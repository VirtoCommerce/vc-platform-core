using VirtoCommerce.NotificationsModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.NotificationsModule.Core.Services
{
    /// <summary>
    /// The registrar is for registration a type in the AbstractTypeFactory
    /// </summary>
    public interface INotificationRegistrar
    {
        void RegisterNotification<T>() where T : Notification;
        void RegisterNotification<T, TMap>() where T : Notification where TMap : AuditableEntity;

        void RegisterNotificationMessage<T>() where T : NotificationMessage;
        void RegisterNotificationMessage<T, TMap>() where T : NotificationMessage where TMap : AuditableEntity;

        void RegisterNotificationTemplate<T>() where T : NotificationTemplate;
        void RegisterNotificationTemplate<T, TMap>() where T : NotificationTemplate where TMap : AuditableEntity;
    }
}
