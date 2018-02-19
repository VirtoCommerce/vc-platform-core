namespace VirtoCommerce.NotificationsModule.Core.Model
{
    public class EmailNotificationMessage : NotificationMessage
    {
        public string From { get; set; }
        public string To { get; set; }
        public string[] CC { get; set; }
        public string[] BCC { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}
