using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VirtoCommerce.NotificationsModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.NotificationsModule.Data.Model
{
    public class EmailNotificationMessageEntity : NotificationMessageEntity
    {
        [NotMapped]
        public override string Kind => nameof(EmailNotification);

        /// <summary>
        /// Subject of notification
        /// </summary>
        [StringLength(512)]
        public string Subject { get; set; }

        /// <summary>
        /// Body of notification
        /// </summary>
        public string Body { get; set; }

        public override NotificationMessage ToModel(NotificationMessage message)
        {
            if (message is EmailNotificationMessage emailNotificationMessage)
            {
                emailNotificationMessage.Subject = Subject;
                emailNotificationMessage.Body = Body;
            }

            return base.ToModel(message);
        }

        public override NotificationMessageEntity FromModel(NotificationMessage message, PrimaryKeyResolvingMap pkMap)
        {
            if (message is EmailNotificationMessage emailNotificationMessage)
            {
                Subject = emailNotificationMessage.Subject;
                Body = emailNotificationMessage.Body;
            }

            return base.FromModel(message, pkMap);
        }

        public override void Patch(NotificationMessageEntity message)
        {
            if (message is EmailNotificationMessageEntity emailNotificationMessageEntity)
            {
                emailNotificationMessageEntity.Subject = Subject;
                emailNotificationMessageEntity.Body = Body;
            }

            base.Patch(message);
        }
    }
}
