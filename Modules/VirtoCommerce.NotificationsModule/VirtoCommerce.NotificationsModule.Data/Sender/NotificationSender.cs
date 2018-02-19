using System.Threading.Tasks;
using VirtoCommerce.NotificationsModule.Core.Abstractions;
using VirtoCommerce.NotificationsModule.Core.Model;

namespace VirtoCommerce.NotificationsModule.Data.Sender
{
    public class NotificationSender : INotificationSender
    {
        private readonly INotificationService _notificationService;

        public NotificationSender(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public async Task SendNotificationAsync(Notification notification, string language)
        {
            var activeNotification = await _notificationService.GetNotificationByTypeAsync(notification.Type);


        }
    }
}
