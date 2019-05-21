using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.NotificationsModule.Core.Model.Search
{
    public class NotificationMessageSearchCriteria : SearchCriteriaBase
    {
        /// <summary>
        /// 
        /// </summary>
        public bool IsActive { get; set; }
        /// <summary>
        /// time interval used to evaluate  active notifications have failure delivery
        /// </summary>
        public int RepeatHoursIntervalForFail { get; set; }
    }
}
