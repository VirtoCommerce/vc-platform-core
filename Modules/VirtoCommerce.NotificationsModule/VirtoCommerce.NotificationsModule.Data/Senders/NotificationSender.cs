using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.NotificationsModule.Core.Abstractions;
using VirtoCommerce.NotificationsModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.NotificationsModule.Data.Senders
{
    public class NotificationSender : INotificationSender
    {
        private readonly INotificationService _notificationService;
        private readonly INotificationTemplateRender _notificationTemplateRender;
        private readonly INotificationMessageService _notificationMessageService;
        private readonly INotificationMessageSender _notificationMessageSender;

        public NotificationSender(INotificationService notificationService, INotificationTemplateRender notificationTemplateRender
            , INotificationMessageService notificationMessageService
            , INotificationMessageSender notificationMessageSender)
        {
            _notificationService = notificationService;
            _notificationTemplateRender = notificationTemplateRender;
            _notificationMessageService = notificationMessageService;
            _notificationMessageSender = notificationMessageSender;
        }

        public async Task SendNotificationAsync(Notification notification, string language)
        {
            var activeNotification = await _notificationService.GetNotificationByTypeAsync(notification.Type);

            var message = AbstractTypeFactory<NotificationMessage>.TryCreateInstance($"{activeNotification.Kind}Message");
            message.LanguageCode = language;
            activeNotification.ToMessage(message, _notificationTemplateRender);

            //todo
            //var subject = _notificationTemplateRender.Render(((EmailNotificationMessage)message).Subject, notification);
            //var body = _notificationTemplateRender.Render(((EmailNotificationMessage)message).Body, notification);

            NotificationMessage[] messages = { message };

            await _notificationMessageService.SaveNotificationMessages(messages);

            var result = await _notificationMessageSender.SendNotificationAsync(message);

            await _notificationMessageService.SaveNotificationMessages(messages);

        }
    }
}
