namespace VirtoCommerce.NotificationsModule.Core.Model
{
    public class SmsNotificationMessage : NotificationMessage
    {
        public string Number { get; set; }
        public string Body { get; set; }
    }
}
