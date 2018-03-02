using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.NotificationsModule.Core.Abstractions;
using VirtoCommerce.NotificationsModule.Core.Extensions;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.NotificationsModule.Core.Model
{
    public class EmailNotification : Notification
    {
        public EmailNotification()
        {
            Kind = nameof(EmailNotification);
            Templates = new List<NotificationTemplate>();
            Attachments = new List<EmailAttachment>();
        }

        public string From { get; set; }
        public string To { get; set; }
        public string[] CC { get; set; }
        public string[] BCC { get; set; }
        public IList<EmailAttachment> Attachments { get; set; }

        public override NotificationMessage ToMessage(NotificationMessage message, INotificationTemplateRender render)
        {
            var emailMessage = (EmailNotificationMessage) message;
            
            var template = (EmailNotificationTemplate)Templates.FindWithLanguage(message.LanguageCode);
            if (template != null)
            {
                emailMessage.Subject = render.Render(template.Subject, this);
                emailMessage.Body = render.Render(template.Body, this);
                emailMessage.From = From;
                emailMessage.To = To;
                emailMessage.CC = CC;
                emailMessage.BCC = BCC;
            }
            

            return base.ToMessage(message, render);
        }
    }
}
