using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VirtoCommerce.NotificationsModule.Core.Abstractions;

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
            var template = (SmsNotificationTemplate)Templates.FirstOrDefault(t => t.LanguageCode.Equals(message.LanguageCode));
            if (template != null)
            {
                smsNotificationMessage.Number = Number;
                smsNotificationMessage.Message = render.Render(template.Message, this);
            }
            
            return base.ToMessage(message, render);
        }
    }
}
