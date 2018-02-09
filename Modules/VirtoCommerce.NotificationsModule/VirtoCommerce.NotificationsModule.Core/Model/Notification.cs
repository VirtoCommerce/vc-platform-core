using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.NotificationsModule.Core.Model
{
    public abstract class Notification : AuditableEntity
    {
        public string TenantId { get; set; }
        public string TenantType { get; set; }
        public bool IsActive { get; set; }
        public string Type { get; set; }
        public string Kind { get; set; }
        public IList<NotificationTemplate> Templates { get; set; }
    }
}
