using System.Threading.Tasks;
using VirtoCommerce.NotificationsModule.Core.Model;

namespace VirtoCommerce.NotificationsModule.Core.Abstractions
{
    public interface INotificationMessageSender
    {
        Task<NotificationSendResult> SendNotificationAsync(NotificationMessage message);
    }
}
