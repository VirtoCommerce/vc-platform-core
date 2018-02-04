using System.Collections.Generic;

namespace VirtoCommerce.NotificationsModule.Web.ViewModels
{
    public class NotificationSearchResult
    {
        public int TotalCount { get; set; }
        public IList<NotificationResult> Results { get; set; }
    }
}
