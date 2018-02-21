using System;
using System.Collections.Generic;
using System.Text;

namespace VirtoCommerce.NotificationsModule.Core.Model
{
    public class SmsNotification : Notification
    {
        public SmsNotification()
        {
            Kind = nameof(SmsNotification);
            Templates = new List<NotificationTemplate>();
        }

        public string Number { get; set; }
    }
}
