using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VirtoCommerce.NotificationsModule.Core.Model;
using VirtoCommerce.NotificationsModule.Data.Model;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Data.Infrastructure;
using VirtoCommerce.Platform.Data.Repositories;

namespace VirtoCommerce.NotificationsModule.Data.Repositories
{
    public class NotificationRepositoryImpl : DbContextRepositoryBase<NotificationDbContext>, INotificationRepository
    {
        public NotificationRepositoryImpl(NotificationDbContext dbContext) : base(dbContext)
        {
        }

        public IQueryable<NotificationEntity> Notifications => DbContext.Set<NotificationEntity>();
        public IQueryable<NotificationMessageEntity> NotifcationMessages => DbContext.Set<NotificationMessageEntity>();

        public Task<NotificationEntity[]> GetNotificationByIds(string[] ids)
        {
            var retVal = Notifications
                .Where(x => ids.Contains(x.Id))
                .OrderBy(x => x.Type)
                .ToArrayAsync();
            return retVal;
        }

        public Task<NotificationEntity> GetNotificationEntityByType(string type, string tenantId, string tenantType)
        {
            return Notifications.SingleAsync(n => n.Type.Equals(type) && n.TenantId.Equals(tenantId) && n.TenantType.Equals(tenantType));
        }

        public Task<NotificationMessageEntity[]> GetNotificationMessageById(string[] ids)
        {
            return NotifcationMessages.Where(x => ids.Contains(x.Id)).ToArrayAsync();
        }

        public void Dispose()
        {
            throw new System.NotImplementedException();
        }

        public IUnitOfWork UnitOfWork { get; }
        public void Attach<T>(T item) where T : class
        {
            throw new System.NotImplementedException();
        }

        public void Add<T>(T item) where T : class
        {
            throw new System.NotImplementedException();
        }

        public void Update<T>(T item) where T : class
        {
            throw new System.NotImplementedException();
        }

        public void Remove<T>(T item) where T : class
        {
            throw new System.NotImplementedException();
        }

        

        
    }
}
