using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VirtoCommerce.NotificationsModule.Data.Model;

namespace VirtoCommerce.NotificationsModule.Data.Abstractions
{
    public interface INotificationManager
    {
        IQueryable<NotificationEntity> GetNotifications();
    }
}
