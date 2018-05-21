namespace VirtoCommerce.NotificationsModule.Core.Model
{
    /// <summary>
    /// Template of email notification
    /// </summary>
    public class EmailNotificationTemplate : NotificationTemplate
    {
        /// <summary>
        /// Subject of Notification
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Body of Notification
        /// </summary>
        public string Body { get; set; }
    }
}
