using System.Threading.Tasks;
using VirtoCommerce.NotificationsModule.Core.Model;

namespace VirtoCommerce.NotificationsModule.Core.Services
{
    /// <summary>
    /// The service is for to make a query to Database and get a list of notifications
    /// </summary>
    public interface INotificationSearchService
    {
        Task<NotificationSearchResult> SearchNotificationsAsync(NotificationSearchCriteria criteria);
    }
}
