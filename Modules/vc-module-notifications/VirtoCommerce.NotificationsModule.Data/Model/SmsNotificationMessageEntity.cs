using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VirtoCommerce.NotificationsModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.NotificationsModule.Data.Model
{
    public class SmsNotificationMessageEntity : NotificationMessageEntity
    {
        [NotMapped]
        public override string Kind => nameof(SmsNotification);

        /// <summary>
        /// Message of notification
        /// </summary>
        [StringLength(1600)]
        public string Message { get; set; }

        /// <summary>
        /// Number for sms
        /// </summary>
        [StringLength(128)]
        public string Number { get; set; }

        public override NotificationMessage ToModel(NotificationMessage message)
        {
            if (message is SmsNotificationMessage smsNotificationMessage)
            {
                smsNotificationMessage.Number = Number;
                smsNotificationMessage.Message = Message;
            }

            return base.ToModel(message);
        }

        public override NotificationMessageEntity FromModel(NotificationMessage message, PrimaryKeyResolvingMap pkMap)
        {
            if (message is SmsNotificationMessage smsNotificationMessage)
            {
                Number = smsNotificationMessage.Number;
                Message = smsNotificationMessage.Message;
            }

            return base.FromModel(message, pkMap);
        }

        public override void Patch(NotificationMessageEntity message)
        {
            if (message is SmsNotificationMessageEntity smsNotificationMessageEntity)
            {
                smsNotificationMessageEntity.Number = Number;
                smsNotificationMessageEntity.Message = Message;
            }

            base.Patch(message);
        }
    }
}
