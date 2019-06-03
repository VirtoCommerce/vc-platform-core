namespace VirtoCommerce.NotificationsModule.Core.Model
{
    /// <summary>
    /// Template of Sms notification
    /// </summary>
    public class SmsNotificationTemplate : NotificationTemplate
    {
        public override string Kind => nameof(SmsNotification);
        /// <summary>
        /// Message of Sms Template Notification
        /// </summary>
        public string Message { get; set; }
    }
}
