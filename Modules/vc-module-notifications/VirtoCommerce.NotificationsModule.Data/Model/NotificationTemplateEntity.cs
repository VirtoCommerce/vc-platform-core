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

            template.Id = Id;
            template.LanguageCode = LanguageCode;
            template.CreatedBy = CreatedBy;
            template.CreatedDate = CreatedDate;
            template.ModifiedBy = ModifiedBy;
            template.ModifiedDate = ModifiedDate;

            switch (template)
            {
                case EmailNotificationTemplate emailNotificationTemplate:
                    emailNotificationTemplate.Subject = Subject;
                    emailNotificationTemplate.Body = Body;
                    break;
                case SmsNotificationTemplate smsNotificationTemplate:
                    smsNotificationTemplate.Message = Message;
                    break;
            }

            return template;
        }

        public virtual NotificationTemplateEntity FromModel(NotificationTemplate template)
        {
            if (template == null) throw new ArgumentNullException(nameof(template));

            Id = template.Id;
            LanguageCode = template.LanguageCode;
            CreatedBy = template.CreatedBy;
            CreatedDate = template.CreatedDate;
            ModifiedBy = template.ModifiedBy;
            ModifiedDate = template.ModifiedDate;

            switch (template)
            {
                case EmailNotificationTemplate emailNotificationTemplate:
                    Subject = emailNotificationTemplate.Subject;
                    Body = emailNotificationTemplate.Body;
                    break;
                case SmsNotificationTemplate smsNotificationTemplate:
                    Message = smsNotificationTemplate.Message;
                    break;
            }

            return this;
        }

        public virtual void Patch(NotificationTemplateEntity template)
        {
            template.LanguageCode = LanguageCode;
            template.Subject = Subject;
            template.Body = Body;
            template.Message = Message;
        }
    }
}
