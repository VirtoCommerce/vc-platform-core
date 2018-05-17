using System.ComponentModel.DataAnnotations;
using VirtoCommerce.NotificationsModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.NotificationsModule.Data.Model
{
    public class SmsNotificationEntity : NotificationEntity
    {
        /// <summary>
        /// Number for sms
        /// </summary>
        [StringLength(128)]
        public string Number { get; set; }

        public override Notification ToModel(Notification notification)
        {
            var smsNotification = notification as SmsNotification;

            if (smsNotification != null)
            {
                smsNotification.Number = Number;
            }

            return base.ToModel(notification);
        }

        public override NotificationEntity FromModel(Notification notification, PrimaryKeyResolvingMap pkMap)
        {
            var smsNotification = notification as SmsNotification;

            if (smsNotification != null)
            {
                Number = smsNotification.Number;
            }

            return base.FromModel(notification, pkMap);
        }

        public override void Patch(NotificationEntity notification)
        {
            var smsNotification = notification as SmsNotificationEntity;

            if (smsNotification != null)
            {
                smsNotification.Number = Number;
            }

            base.Patch(notification);
        }
    }
}
