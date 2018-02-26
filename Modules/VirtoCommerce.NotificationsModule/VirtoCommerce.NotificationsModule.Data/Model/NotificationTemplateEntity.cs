using System;
using System.ComponentModel.DataAnnotations;
using VirtoCommerce.NotificationsModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.NotificationsModule.Data.Model
{
    public class NotificationTemplateEntity : AuditableEntity
    {
        /// <summary>
        /// Language
        /// </summary>
        [StringLength(10)]
        public string LanguageCode { get; set; }

        /// <summary>
        /// Subject of notification
        /// </summary>
        [StringLength(512)]
        public string Subject { get; set; }

        /// <summary>
        /// Body of notification
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// Message of notification
        /// </summary>
        [StringLength(1600)]
        public string Message { get; set; }

        [StringLength(128)]
        public string NotificationId { get; set; }

        public virtual NotificationTemplate ToModel(NotificationTemplate message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));

            message.Id = this.Id;
            message.LanguageCode = this.LanguageCode;
            message.CreatedBy = this.CreatedBy;
            message.CreatedDate = this.CreatedDate;
            message.ModifiedBy = this.ModifiedBy;
            message.ModifiedDate = this.ModifiedDate;

            switch (message)
            {
                case EmailNotificationTemplate emailNotificationTemplate:
                    emailNotificationTemplate.Subject = this.Subject;
                    emailNotificationTemplate.Body = this.Body;
                    break;
                case SmsNotificationTemplate smsNotificationTemplate:
                    smsNotificationTemplate.Message = this.Message;
                    break;
            }

            return message;
        }

        public virtual NotificationTemplateEntity FromModel(NotificationTemplate message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));

            this.Id = message.Id;
            this.LanguageCode = message.LanguageCode;
            this.CreatedBy = message.CreatedBy;
            this.CreatedDate = message.CreatedDate;
            this.ModifiedBy = message.ModifiedBy;
            this.ModifiedDate = message.ModifiedDate;

            switch (message)
            {
                case EmailNotificationTemplate emailNotificationTemplate:
                    this.Subject = emailNotificationTemplate.Subject;
                    this.Body = emailNotificationTemplate.Body;
                    break;
                case SmsNotificationTemplate smsNotificationTemplate:
                    this.Message = smsNotificationTemplate.Message;
                    break;
            }

            return this;
        }

        public virtual void Patch(NotificationTemplateEntity template)
        {
            template.LanguageCode = this.LanguageCode;
            template.Subject = this.Subject;
            template.Body = this.Body;
            template.Message = this.Message;
        }
    }
}
