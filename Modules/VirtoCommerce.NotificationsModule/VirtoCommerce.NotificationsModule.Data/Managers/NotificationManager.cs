using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VirtoCommerce.NotificationsModule.Data.Abstractions;
using VirtoCommerce.NotificationsModule.Data.Model;

namespace VirtoCommerce.NotificationsModule.Data.Managers
{
    public class NotificationManager : INotificationManager
    {


        public IQueryable<NotificationEntity> GetNotifications()
        {
            throw new NotImplementedException();
        }
    }
}
