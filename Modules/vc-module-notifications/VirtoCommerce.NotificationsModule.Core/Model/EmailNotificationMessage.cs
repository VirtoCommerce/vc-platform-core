using System.Collections.Generic;

namespace VirtoCommerce.NotificationsModule.Core.Model
{
    /// <summary>
    /// Email - Kind of message notification
    /// </summary>
    public class EmailNotificationMessage : NotificationMessage
    {
        public EmailNotificationMessage()
        {
            Attachments = new List<EmailAttachment>();
        }

        /// <summary>
        /// Sender of Notification
        /// </summary>
        public string From { get; set; }

        /// <summary>
        /// Recipient of Notification
        /// </summary>
        public string To { get; set; }

        /// <summary>
        /// Array of CC recipients
        /// </summary>
        public string[] CC { get; set; }

        /// <summary>
        /// Array of BCC recipients
        /// </summary>
        public string[] BCC { get; set; }

        /// <summary>
        /// Subject of Notification
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Body of Notification
        /// </summary>
        public string Body { get; set; }

        public IList<EmailAttachment> Attachments { get; set; }
    }
}
