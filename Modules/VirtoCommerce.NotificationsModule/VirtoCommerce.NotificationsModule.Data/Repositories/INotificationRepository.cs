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
        Task<NotificationEntity> GetNotificationEntityByType(string type, string tenantId, string tenantType);
        Task<NotificationEntity[]> GetNotificationByIds(string[] ids);
        Task<NotificationMessageEntity[]> GetNotificationMessageById(string[] ids);
    }
}
