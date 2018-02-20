using System.Threading.Tasks;
using VirtoCommerce.NotificationsModule.Core.Model;

namespace VirtoCommerce.NotificationsModule.Notifications.Senders
{
    public interface INotificationSender
    {
        Task SendNotificationAsync(Notification notification, string language);
    }
}
