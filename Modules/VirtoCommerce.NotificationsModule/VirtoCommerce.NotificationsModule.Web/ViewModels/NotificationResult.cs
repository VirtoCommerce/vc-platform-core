namespace VirtoCommerce.NotificationsModule.Web.ViewModels
{
    public class NotificationResult
    {
        public string Id { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public string SendGatewayType { get; set; }
        public string NotificationType { get; set; }
        public bool IsActive { get; set; }
        public bool IsSuccessSend { get; set; }
        public int AttemptCount { get; set; }
        public int MaxAttemptCount { get; set; }
    }
}
