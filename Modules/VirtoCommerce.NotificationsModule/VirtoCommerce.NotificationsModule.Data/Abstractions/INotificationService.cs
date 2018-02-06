using System;
using System.Collections.Generic;
using System.Text;
using VirtoCommerce.NotificationsModule.Data.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.NotificationsModule.Data.Abstractions
{
    public interface INotificationService
    {
        GenericSearchResult<NotificationResult> GetNotifications();
        NotificationResult GetNotificationByTypeId(string typeId);
        void UpdateNotification(Notification notification);
        void DeleteNotification(string id);
        GenericSearchResult<NotificationResult> SearchNotifications(NotificationSearchCriteria criteria);
    }
}
