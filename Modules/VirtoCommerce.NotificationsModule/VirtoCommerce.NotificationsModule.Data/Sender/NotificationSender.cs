using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.NotificationsModule.Core.Abstractions;
using VirtoCommerce.NotificationsModule.Core.Model;

namespace VirtoCommerce.NotificationsModule.Data.Sender
{
    public class NotificationSender : INotificationSender
    {
        private readonly INotificationService _notificationService;
        private readonly INotificationTemplateRender _notificationTemplateRender;
        private readonly INotificationMessageService _notificationMessageService;
        private readonly INotificationMessageSender _notificationMessageSender;

        public NotificationSender(INotificationService notificationService, INotificationTemplateRender notificationTemplateRender
            , INotificationMessageService notificationMessageService)
        {
            _notificationService = notificationService;
            _notificationTemplateRender = notificationTemplateRender;
            _notificationMessageService = notificationMessageService;
        }

        public async Task SendNotificationAsync(Notification notification, string language)
        {
            var activeNotification = await _notificationService.GetNotificationByTypeAsync(notification.Type);

            switch (activeNotification)
            {
                case EmailNotification emailNotification:
                    var template = (EmailNotificationTemplate)emailNotification.Templates.Single(t => t.LanguageCode.Equals(language));
                    var subject = _notificationTemplateRender.Render(template.Subject, emailNotification);
                    var body = _notificationTemplateRender.Render(template.Body, emailNotification);

                    //todo 
                    var message = new EmailNotificationMessage()
                    {
                        From = emailNotification.From,
                        To = emailNotification.To,
                        CC = emailNotification.CC.Select(cc => cc.Value).ToArray(),
                        BCC = emailNotification.BCC.Select(bcc => bcc.Value).ToArray(),
                        // Attachments
                        Subject = subject,
                        Body = body,

                    };
                    _notificationMessageService.SaveNotificationMessages(new NotificationMessage[] { message });

                    await _notificationMessageSender.SendNotificationAsync(message);

                    _notificationMessageService.SaveNotificationMessages(new NotificationMessage[] { message });
                    break;
                case SmsNotification smsNotification:
                    //smsNotification.Number = this.Number;
                    break;
            }

            
        }
    }
}
