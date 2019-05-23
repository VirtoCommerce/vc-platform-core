using System.Threading.Tasks;
using VirtoCommerce.NotificationsModule.Core.Model;

namespace VirtoCommerce.NotificationsModule.Core.Services
{
    /// <summary>
    /// The service has a logic query/save of notification to Database
    /// </summary>
    public interface INotificationService
    {
        Task<Notification[]> GetByIdsAsync(string[] ids, string responseGroup = null);
        Task SaveChangesAsync(Notification[] notifications);
    }
}
