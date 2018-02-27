using System.Collections.Generic;
using System.Linq;
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

        public override NotificationMessage ToMessage(NotificationMessage message)
        {
            var emailMessage = (EmailNotificationMessage) message;
            
            var template = (EmailNotificationTemplate)Templates.FirstOrDefault(t => t.LanguageCode.Equals(message.LanguageCode));
            if (template != null)
            {
                emailMessage.Body = template.Body;
                emailMessage.Subject = template.Subject;
                emailMessage.From = From;
                emailMessage.To = To;
                emailMessage.CC = CC;
                emailMessage.BCC = BCC;
            }
            

            return base.ToMessage(message);
        }
    }
}
