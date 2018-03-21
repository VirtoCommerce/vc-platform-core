using System.Threading.Tasks;
using VirtoCommerce.NotificationsModule.Core.Model;

namespace VirtoCommerce.NotificationsModule.Core.Services
{
    /// <summary>
    /// The service has a logic for preparing/sending a message with error handling and throttling
    /// </summary>
    public interface INotificationSender
    {
        Task<NotificationSendResult> SendNotificationAsync(Notification notification, string language);
    }
}
