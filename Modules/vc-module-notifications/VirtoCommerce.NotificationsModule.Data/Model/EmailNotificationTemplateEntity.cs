using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VirtoCommerce.NotificationsModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.NotificationsModule.Data.Model
{
    public class EmailNotificationTemplateEntity : NotificationTemplateEntity
    {
        [NotMapped]
        public override string Kind => nameof(EmailNotification);

        /// <summary>
        /// Subject of template
        /// </summary>
        [StringLength(512)]
        public string Subject { get; set; }

        /// <summary>
        /// Body of template
        /// </summary>
        public string Body { get; set; }

        public override NotificationTemplate ToModel(NotificationTemplate template)
        {
            if (template is EmailNotificationTemplate emailNotificationTemplate)
            {
                emailNotificationTemplate.Subject = Subject;
                emailNotificationTemplate.Body = Body;
            }

            return base.ToModel(template);
        }

        public override NotificationTemplateEntity FromModel(NotificationTemplate template, PrimaryKeyResolvingMap pkMap)
        {
            if (template is EmailNotificationTemplate emailNotificationTemplate)
            {
                Subject = emailNotificationTemplate.Subject;
                Body = emailNotificationTemplate.Body;
            }

            return base.FromModel(template, pkMap);
        }

        public override void Patch(NotificationTemplateEntity target)
        {
            if (target is EmailNotificationTemplateEntity emailNotificationTemplateEntity)
            {
                emailNotificationTemplateEntity.Subject = Subject;
                emailNotificationTemplateEntity.Body = Body;
            }

            base.Patch(target);
        }
    }
}
