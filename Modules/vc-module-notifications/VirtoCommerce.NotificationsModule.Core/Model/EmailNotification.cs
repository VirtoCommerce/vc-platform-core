using System.Collections.Generic;
using VirtoCommerce.NotificationsModule.Core.Extensions;
using VirtoCommerce.NotificationsModule.Core.Services;

namespace VirtoCommerce.NotificationsModule.Core.Model
{
    /// <summary>
    /// Type of Notification for the Email
    /// </summary>
    public abstract class EmailNotification : Notification
    {
        public EmailNotification()
        {
            Type = GetType().Name;
            Templates = new List<NotificationTemplate>();
            Attachments = new List<EmailAttachment>();
        }

        public override string Kind => nameof(EmailNotification);

        /// <summary>
        /// Sender
        /// </summary>
        public string From { get; set; }

        /// <summary>
        /// Recipient
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
        public IList<EmailAttachment> Attachments { get; set; }

        public override NotificationMessage ToMessage(NotificationMessage message, INotificationTemplateRenderer render)
        {
            var emailMessage = (EmailNotificationMessage)message;

            var template = (EmailNotificationTemplate)Templates.FindWithLanguage(message.LanguageCode);
            if (template != null)
            {
                emailMessage.Subject = render.Render(template.Subject, this);
                emailMessage.Body = render.Render(template.Body, this);
                emailMessage.From = From;
                emailMessage.To = To;
                emailMessage.CC = CC;
                emailMessage.BCC = BCC;
                emailMessage.Attachments = Attachments;
            }

            return base.ToMessage(message, render);
        }
    }
}
