using System.Threading.Tasks;
using VirtoCommerce.NotificationsModule.Core.Model;

namespace VirtoCommerce.NotificationsModule.Notifications.Senders
{
    public interface INotificationMessageSender
    {
        Task SendNotificationAsync(NotificationMessage message);
    }
}
