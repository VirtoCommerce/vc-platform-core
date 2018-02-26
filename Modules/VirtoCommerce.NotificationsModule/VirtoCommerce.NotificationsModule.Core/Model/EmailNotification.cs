using System.Collections.Generic;
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
    }
}
