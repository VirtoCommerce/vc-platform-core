namespace VirtoCommerce.OrdersModule.Core.Notifications
{
    public class NewOrderStatusEmailNotification : OrderEmailNotificationBase
    {
        public string OldStatus { get; set; }
        public string NewStatus { get; set; }
    }
}
