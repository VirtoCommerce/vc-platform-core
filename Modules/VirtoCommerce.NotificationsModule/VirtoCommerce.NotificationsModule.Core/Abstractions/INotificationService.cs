using System.Threading.Tasks;
using VirtoCommerce.NotificationsModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.NotificationsModule.Core.Abstractions
{
    public interface INotificationService
    {
        Task<Notification> GetNotificationByType(string type, string tenantId = null);
        Task<Notification[]> GetNotificationsByIds(string ids);
        void SaveChanges(Notification[] notifications);
    }
}
