namespace VirtoCommerce.NotificationsModule.Core.Model
{
    /// <summary>
    /// Template of Sms notification
    /// </summary>
    public class SmsNotificationTemplate : NotificationTemplate
    {
        /// <summary>
        /// Message of Sms Template Notification
        /// </summary>
        public string Message { get; set; }
    }
}
