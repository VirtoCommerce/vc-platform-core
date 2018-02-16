using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.NotificationsModule.Core.Model
{
    public class EmailNotification : Notification
    {
        public EmailNotification()
        {
            Kind = nameof(EmailNotification);
            Attachments = new List<EmailAttachment>();
        }

        public string From { get; set; }
        public string To { get; set; }
        public EmailAddress[] CC { get; set; }
        public EmailAddress[] BCC { get; set; }
        public IList<EmailAttachment> Attachments { get; set; }
    }

    public class EmailAddress : AuditableEntity
    {
        public string Value { get; set; }
    }
}
