using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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

        public override void ToMessage(NotificationMessage message, INotificationTemplateRenderer render)
        {
            base.ToMessage(message, render);

            var emailMessage = (EmailNotificationMessage)message;

            var template = (EmailNotificationTemplate)Templates.FindWithLanguage(message.LanguageCode);
            if (template != null)
            {
                emailMessage.Subject = render.RenderAsync(template.Subject, this, template.LanguageCode).GetAwaiter().GetResult();
                emailMessage.Body = render.RenderAsync(template.Body, this, template.LanguageCode).GetAwaiter().GetResult();
            }

            emailMessage.From = From;
            emailMessage.To = To;
            emailMessage.CC = CC;
            emailMessage.BCC = BCC;
            emailMessage.Attachments = Attachments;
        }

        public override void SetFromToMembers(string from, string to)
        {
            From = from;
            To = to;
        }

        #region ICloneable members

        public override object Clone()
        {
            var result = base.Clone() as EmailNotification;

            result.Attachments = Attachments?.Select(x => x.Clone()).OfType<EmailAttachment>().ToList();

            return result;
        }

        #endregion
    }
}
