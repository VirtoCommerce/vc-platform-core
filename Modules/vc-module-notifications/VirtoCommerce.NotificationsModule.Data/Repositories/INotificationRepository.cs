using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.NotificationsModule.Core.Model;
using VirtoCommerce.NotificationsModule.Data.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.NotificationsModule.Data.Repositories
{
    public interface INotificationRepository : IRepository
    {
        IQueryable<NotificationEntity> Notifications { get; }
        IQueryable<NotificationMessageEntity> NotifcationMessages { get; }
        Task<NotificationEntity> GetByTypeAsync(string type, string tenantId, string tenantType, NotificationResponseGroup responseGroup);
        Task<NotificationEntity[]> GetByIdsAsync(string[] ids, NotificationResponseGroup responseGroup);
        Task<NotificationMessageEntity[]> GetMessageByIdAsync(string[] ids);
    }
}
