using System.Threading.Tasks;
using VirtoCommerce.NotificationsModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.NotificationsModule.Core.Abstractions
{
    public interface INotificationSearchService
    {
        Task<GenericSearchResult<Notification>> SearchNotificationsAsync(NotificationSearchCriteria criteria);
    }
}
