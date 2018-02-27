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
            activeNotification.ToMessage(message);

            NotificationMessage[] messages = { message };

            await _notificationMessageService.SaveNotificationMessages(messages);

            var result = await _notificationMessageSender.SendNotificationAsync(message);

            await _notificationMessageService.SaveNotificationMessages(messages);


            //switch (activeNotification)
            //{
            //    case EmailNotification emailNotification:
            //        var template = (EmailNotificationTemplate)emailNotification.Templates.Single(t => t.LanguageCode.Equals(language));
            //        var subject = _notificationTemplateRender.Render(template.Subject, notification);
            //        var body = _notificationTemplateRender.Render(template.Body, emailNotification);

            //        //todo 
            //        var message = new EmailNotificationMessage()
            //        {
            //            From = emailNotification.From,
            //            To = emailNotification.To,
            //            CC = emailNotification.CC,
            //            BCC = emailNotification.BCC,
            //            // Attachments
            //            Subject = subject,
            //            Body = body,
            //        };
            //        NotificationMessage[] messages = { message };
            //        await _notificationMessageService.SaveNotificationMessages(messages);

            //        var result = await _notificationMessageSender.SendNotificationAsync(message);

            //        await _notificationMessageService.SaveNotificationMessages(messages);
            //        break;
            //    case SmsNotification smsNotification:
            //        //smsNotification.Number = this.Number;
            //        break;
            //}


        }
    }
}
