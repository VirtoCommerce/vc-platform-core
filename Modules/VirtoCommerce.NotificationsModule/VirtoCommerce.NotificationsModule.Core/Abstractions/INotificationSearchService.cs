using VirtoCommerce.NotificationsModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.NotificationsModule.Core.Abstractions
{
    public interface INotificationSearchService
    {
        GenericSearchResult<Notification> SearchNotifications(NotificationSearchCriteria criteria);
    }
}
