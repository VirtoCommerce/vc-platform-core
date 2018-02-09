using System;
using System.Collections.Generic;
using System.Text;

namespace VirtoCommerce.NotificationsModule.Core.Abstractions
{
    public interface INotificationRegistrar
    {
        void RegisterNotification<T>();
    }
}
