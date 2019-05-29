using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VirtoCommerce.NotificationsModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.NotificationsModule.Data.Model
{
    public class SmsNotificationTemplateEntity : NotificationTemplateEntity
    {
        [NotMapped]
        public override string Kind => nameof(SmsNotification);
        /// <summary>
        /// Message of template
        /// </summary>
        [StringLength(1600)]
        public string Message { get; set; }


        public override NotificationTemplate ToModel(NotificationTemplate template)
        {
            if (template is SmsNotificationTemplate smsNotificationTemplate)
            {
                smsNotificationTemplate.Message = Message;
            }

            return base.ToModel(template);
        }

        public override NotificationTemplateEntity FromModel(NotificationTemplate template, PrimaryKeyResolvingMap pkMap)
        {
            if (template is SmsNotificationTemplate smsNotificationTemplate)
            {
                Message = smsNotificationTemplate.Message;
            }

            return base.FromModel(template, pkMap);
        }

        public override void Patch(NotificationTemplateEntity target)
        {
            if (target is SmsNotificationTemplateEntity smsNotificationTemplateEntity)
            {
                smsNotificationTemplateEntity.Message = Message;
            }

            base.Patch(target);
        }
    }
}
