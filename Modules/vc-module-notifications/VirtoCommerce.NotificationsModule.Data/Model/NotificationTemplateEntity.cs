using System;
using System.ComponentModel.DataAnnotations;
using VirtoCommerce.NotificationsModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.NotificationsModule.Data.Model
{
    /// <summary>
    /// Entity is template of Notification
    /// </summary>
    public class NotificationTemplateEntity : AuditableEntity
    {
        /// <summary>
        /// Language of template
        /// </summary>
        [StringLength(10)]
        public string LanguageCode { get; set; }

        /// <summary>
        /// Subject of template
        /// </summary>
        [StringLength(512)]
        public string Subject { get; set; }

        /// <summary>
        /// Body of template
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// Message of template
        /// </summary>
        [StringLength(1600)]
        public string Message { get; set; }

        /// <summary>
        /// Id of notification
        /// </summary>
        [StringLength(128)]
        public string NotificationId { get; set; }

        public virtual NotificationTemplate ToModel(NotificationTemplate template)
        {
            if (template == null) throw new ArgumentNullException(nameof(template));

            template.Id = this.Id;
            template.LanguageCode = this.LanguageCode;
            template.CreatedBy = this.CreatedBy;
            template.CreatedDate = this.CreatedDate;
            template.ModifiedBy = this.ModifiedBy;
            template.ModifiedDate = this.ModifiedDate;

            switch (template)
            {
                case EmailNotificationTemplate emailNotificationTemplate:
                    emailNotificationTemplate.Subject = this.Subject;
                    emailNotificationTemplate.Body = this.Body;
                    break;
                case SmsNotificationTemplate smsNotificationTemplate:
                    smsNotificationTemplate.Message = this.Message;
                    break;
            }

            return template;
        }

        public virtual NotificationTemplateEntity FromModel(NotificationTemplate template)
        {
            if (template == null) throw new ArgumentNullException(nameof(template));

            this.Id = template.Id;
            this.LanguageCode = template.LanguageCode;
            this.CreatedBy = template.CreatedBy;
            this.CreatedDate = template.CreatedDate;
            this.ModifiedBy = template.ModifiedBy;
            this.ModifiedDate = template.ModifiedDate;

            switch (template)
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
