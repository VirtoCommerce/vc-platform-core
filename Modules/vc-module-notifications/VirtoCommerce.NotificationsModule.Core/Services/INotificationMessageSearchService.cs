using System.Threading.Tasks;
using VirtoCommerce.NotificationsModule.Core.Model.Search;

namespace VirtoCommerce.NotificationsModule.Core.Services
{
    public interface INotificationMessageSearchService
    {
        Task<NotificationMessageSearchResult> SearchMessageAsync(NotificationMessageSearchCriteria criteria);
    }
}
