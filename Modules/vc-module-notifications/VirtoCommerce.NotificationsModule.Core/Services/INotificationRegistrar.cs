using System;
using VirtoCommerce.NotificationsModule.Core.Model;

namespace VirtoCommerce.NotificationsModule.Core.Services
{
    /// <summary>
    /// The registrar is for registration a type in the AbstractTypeFactory
    /// </summary>
    public interface INotificationRegistrar
    {
        NotificationBuilder RegisterNotification<T>(Func<Notification> factory = null) where T : Notification;

        NotificationBuilder OverrideNotificationType<OldType, NewType>(Func<Notification> factory = null) where OldType : Notification where NewType : Notification;
    }
}
