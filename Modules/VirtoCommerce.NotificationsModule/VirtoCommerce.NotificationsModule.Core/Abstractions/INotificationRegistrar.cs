using System;
using System.Collections.Generic;
using System.Text;
using VirtoCommerce.NotificationsModule.Core.Model;

namespace VirtoCommerce.NotificationsModule.Core.Abstractions
{
    /// <summary>
    /// The registrar is for registration a type in the AbstractTypeFactory
    /// </summary>
    public interface INotificationRegistrar 
    {
        void RegisterNotification<T>() where T : Notification;
        void RegisterNotification<T>(T type) where T : Notification;
    }
}
