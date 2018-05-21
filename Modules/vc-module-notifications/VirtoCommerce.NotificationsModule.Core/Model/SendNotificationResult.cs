namespace VirtoCommerce.NotificationsModule.Core.Model
{
    /// <summary>
    /// Result of notification sending
    /// </summary>
    public class NotificationSendResult
    {
        public string ErrorMessage { get; set; }
        public bool IsSuccess { get; set; }
    }
}
