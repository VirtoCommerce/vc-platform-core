using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.NotificationsModule.Core.Model
{
    /// <summary>
    /// Criteria for searching
    /// </summary>
    public class NotificationSearchCriteria : SearchCriteriaBase
    {
        public string NotificationType { get; set; }
        /// <summary>
        /// Owner Id of Notification
        /// </summary>
        public string TenantId { get; set; }

        /// <summary>
        /// Owner Type of Notification
        /// </summary>
        public string TenantType { get; set; }

        /// <summary>
        /// only active notification
        /// </summary>
        public bool IsActive { get; set; }
    }
}
