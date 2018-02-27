namespace VirtoCommerce.NotificationsModule.Core.Model
{
    public class SmsNotificationMessage : NotificationMessage
    {
        public string Number { get; set; }
        public string Message { get; set; }
    }
}
