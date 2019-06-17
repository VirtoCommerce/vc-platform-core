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
        Task<NotificationEntity[]> GetByIdsAsync(string[] ids, string responseGroup);
        Task<NotificationMessageEntity[]> GetMessagesByIdsAsync(string[] ids);
    }
}
