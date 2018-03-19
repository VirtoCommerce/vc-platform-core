using System.Collections.Generic;
using VirtoCommerce.NotificationsModule.Core.Abstractions;
using VirtoCommerce.NotificationsModule.Core.Extensions;

namespace VirtoCommerce.NotificationsModule.Core.Model
{
    public class SmsNotification : Notification
    {
        public SmsNotification()
        {
            Kind = nameof(SmsNotification);
            Templates = new List<NotificationTemplate>();
        }

        public string Number { get; set; }

        public override NotificationMessage ToMessage(NotificationMessage message, INotificationTemplateRender render)
        {
            var smsNotificationMessage = (SmsNotificationMessage) message;
            var template = (SmsNotificationTemplate)Templates.FindWithLanguage(message.LanguageCode);
            if (template != null)
            {
                smsNotificationMessage.Number = Number;
                smsNotificationMessage.Message = render.Render(template.Message, this);
            }
            
            return base.ToMessage(message, render);
        }
    }
}
