using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.NotificationsModule.Data.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.NotificationsModule.Data.Repositories
{
    public interface INotificationRepository : IRepository
    {
        IQueryable<NotificationEntity> Notifications { get; }
        IQueryable<NotificationMessageEntity> NotifcationMessages { get; }
        Task<NotificationEntity> GetByTypeAsync(string type, string tenantId, string tenantType);
        NotificationEntity GetEntityForListByType(string type, string tenantId, string tenantType);
        Task<NotificationEntity[]> GetByIdsAsync(string[] ids);
        Task<NotificationMessageEntity[]> GetMessageByIdAsync(string[] ids);
    }
}
