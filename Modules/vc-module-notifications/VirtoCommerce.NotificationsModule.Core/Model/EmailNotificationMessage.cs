using System.Collections.Generic;

namespace VirtoCommerce.NotificationsModule.Core.Model
{
    public class EmailNotificationMessage : NotificationMessage
    {
        public EmailNotificationMessage()
        {
            Attachments = new List<EmailAttachment>();
        }
        public string From { get; set; }
        public string To { get; set; }
        public string[] CC { get; set; }
        public string[] BCC { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }

        public IList<EmailAttachment> Attachments { get; set; }
    }
}
