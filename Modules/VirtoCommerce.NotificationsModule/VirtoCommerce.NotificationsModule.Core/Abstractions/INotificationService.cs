using System.Threading.Tasks;
using VirtoCommerce.NotificationsModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.NotificationsModule.Core.Abstractions
{
    public interface INotificationService
    {
        Task<Notification> GetNotificationByTypeAsync(string type, string tenantId = null);
        Task<Notification[]> GetNotificationsByIdsAsync(string ids);
        Task SaveChangesAsync(Notification[] notifications);
    }
}
