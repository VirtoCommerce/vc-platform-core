using System.Threading.Tasks;
using VirtoCommerce.NotificationsModule.Core.Model;

namespace VirtoCommerce.NotificationsModule.Core.Services
{
    /// <summary>
    /// The service has a logic query/save of message to Database
    /// </summary>
    public interface INotificationMessageService
    {
        Task<NotificationMessage[]> GetNotificationsMessageByIds(string[] ids);
        Task SaveNotificationMessagesAsync(NotificationMessage[] messages);
    }
}
