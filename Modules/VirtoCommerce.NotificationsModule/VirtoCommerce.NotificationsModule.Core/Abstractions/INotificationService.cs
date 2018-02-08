using VirtoCommerce.NotificationsModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.NotificationsModule.Core.Abstractions
{
    public interface INotificationService
    {
        GenericSearchResult<Notification> GetNotifications();
        Notification GetNotificationByTypeId(string typeId);
        void UpdateNotification(Notification notification);
        void DeleteNotification(string id);
        GenericSearchResult<Notification> SearchNotifications(NotificationSearchCriteria criteria);
    }
}
