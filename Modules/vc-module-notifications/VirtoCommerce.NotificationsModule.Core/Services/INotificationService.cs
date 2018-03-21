using System.Threading.Tasks;
using VirtoCommerce.NotificationsModule.Core.Model;

namespace VirtoCommerce.NotificationsModule.Core.Services
{
    /// <summary>
    /// The service has a logic query/save of notification to Database
    /// </summary>
    public interface INotificationService
    {
        Task<Notification> GetNotificationByTypeAsync(string type, string tenantId = null, string tenantType = null);
        Task<Notification[]> GetNotificationsByIdsAsync(string[] ids);
        Task SaveChangesAsync(Notification[] notifications);
    }
}
