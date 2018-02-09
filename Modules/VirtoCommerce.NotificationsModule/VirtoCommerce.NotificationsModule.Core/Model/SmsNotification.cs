using System;
using System.Collections.Generic;
using System.Text;

namespace VirtoCommerce.NotificationsModule.Core.Model
{
    public class SmsNotification : Notification
    {
        public SmsNotification()
        {
            Kind = "SMS";
        }

        public string Number { get; set; }
    }
}
