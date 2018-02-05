using System;
using System.Collections.Generic;
using System.Text;
using VirtoCommerce.NotificationsModule.Core.Model;

namespace VirtoCommerce.NotificationsModule.Data.Abstractions
{
    public interface INotificationService
    {
        GenericSearchResult<NotificationResult> GetNotifications();
        NotificationResult GetNotificationByTypeId(string typeId);
        void UpdateNotification(NotificationResult notification);
        void DeleteNotification(string id);
        //SearchNotificationsResult SearchNotifications(SearchNotificationCriteria criteria);
    }
}
