using VirtoCommerce.NotificationsModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.NotificationsModule.Core.Abstractions
{
    public interface INotificationService
    {
        Notification GetNotificationByType(string type, string tenantId = null);
        Notification[] GetNotificationsByIds(string ids);
        void SaveChanges(Notification[] notifications);
    }
}
