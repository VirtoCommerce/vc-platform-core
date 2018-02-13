using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VirtoCommerce.NotificationsModule.Data.Model;
using VirtoCommerce.Platform.Data.Infrastructure;

namespace VirtoCommerce.NotificationsModule.Data.Repositories
{
    public class NotificationRepositoryImpl : DbContextRepositoryBase<NotificationDbContext>, INotificationRepository
    {
        public NotificationRepositoryImpl(NotificationDbContext dbContext) : base(dbContext)
        {
        }

        public IQueryable<NotificationEntity> Notifications => DbContext.Set<NotificationEntity>();
        public IQueryable<NotificationMessageEntity> NotifcationMessages => DbContext.Set<NotificationMessageEntity>();

        public Task<NotificationEntity[]> GetNotificationByIdsAsync(string[] ids)
        {
            return Notifications
                .Where(x => ids.Contains(x.Id))
                .OrderBy(x => x.Type)
                .ToArrayAsync();
        }

        public Task<NotificationEntity> GetNotificationEntityByTypeAsync(string type, string tenantId, string tenantType)
        {
            return Notifications.SingleAsync(n => n.Type.Equals(type) /*&& n.TenantId.Equals(tenantId) && n.TenantType.Equals(tenantType)*/);
        }

        public Task<NotificationMessageEntity[]> GetNotificationMessageByIdAsync(string[] ids)
        {
            return NotifcationMessages.Where(x => ids.Contains(x.Id)).ToArrayAsync();
        }
    }
}
