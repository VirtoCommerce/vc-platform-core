using VirtoCommerce.NotificationsModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.NotificationsModule.Core.Services
{
    /// <summary>
    /// The service is for to make a query to Database and get a list of notifications
    /// </summary>
    public interface INotificationSearchService
    {
        GenericSearchResult<Notification> SearchNotifications(NotificationSearchCriteria criteria);
    }
}
