using System;
using System.Collections.Generic;
using System.Text;
using VirtoCommerce.NotificationsModule.Core.Abstractions;
using VirtoCommerce.NotificationsModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.NotificationsModule.Data.Services
{
    public class NotificationRegistrar : INotificationRegistrar
    {
        public void RegisterNotification<T>()
        {
            AbstractTypeFactory<T>.RegisterType<T>();
        }
    }
}
