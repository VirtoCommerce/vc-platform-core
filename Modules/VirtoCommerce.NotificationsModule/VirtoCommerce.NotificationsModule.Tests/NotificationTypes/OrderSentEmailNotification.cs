using System;
using VirtoCommerce.NotificationsModule.Core.Model;

namespace VirtoCommerce.NotificationsModule.Tests.NotificationTypes
{
    public class OrderSentEmailNotification : EmailNotification
    {
        public CustomerOrder Order { get; set; }
    }

    public class CustomerOrder
    {
        public string Id { get; set; }
        public decimal ShippingTotal { get; set; }
        public string Number { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
