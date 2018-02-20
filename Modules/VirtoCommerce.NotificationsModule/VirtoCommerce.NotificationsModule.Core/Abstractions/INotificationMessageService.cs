using System.Threading.Tasks;
using VirtoCommerce.NotificationsModule.Core.Model;

namespace VirtoCommerce.NotificationsModule.Core.Abstractions
{
    public interface INotificationMessageService
    {
        Task<NotificationMessage[]> GetNotificationsMessageByIds(string[] ids);
        Task SaveNotificationMessages(NotificationMessage[] messages);
    }
}
