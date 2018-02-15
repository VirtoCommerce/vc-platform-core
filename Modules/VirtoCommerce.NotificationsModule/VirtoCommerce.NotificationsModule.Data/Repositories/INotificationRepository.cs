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
        Task<NotificationEntity> GetNotificationEntityByTypeAsync(string type, string tenantId, string tenantType);
        Task<NotificationEntity[]> GetNotificationByIdsAsync(string[] ids);
        Task<NotificationMessageEntity[]> GetNotificationMessageByIdAsync(string[] ids);
        //Task<int> Update(NotificationEntity notification);
        //Task<int> Add(NotificationEntity notification);
    }
}
