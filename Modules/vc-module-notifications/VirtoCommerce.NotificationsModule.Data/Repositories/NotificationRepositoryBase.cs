using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VirtoCommerce.NotificationsModule.Data.Model;
using VirtoCommerce.Platform.Data.Infrastructure;

namespace VirtoCommerce.NotificationsModule.Data.Repositories
{
    public abstract class NotificationRepositoryBase : DbContextRepositoryBase<NotificationDbContext>, INotificationRepository
    {
        protected NotificationRepositoryBase(NotificationDbContext dbContext) : base(dbContext)
        {
        }

        public IQueryable<NotificationEntity> Notifications => DbContext.Set<NotificationEntity>();
        public IQueryable<NotificationMessageEntity> NotifcationMessages => DbContext.Set<NotificationMessageEntity>();

        public async Task<NotificationEntity[]> GetByIdsAsync(string[] ids)
        {
            var notifications = await Notifications
                .Where(x => ids.Contains(x.Id))
                .OrderBy(x => x.Type)
                .ToArrayAsync();

            foreach (var notification in notifications)
            {
                var templatesTask = DbContext.Set<NotificationTemplateEntity>().Where(t => t.NotificationId.Equals(notification.Id)).ToListAsync();
                var attachmentsTask = DbContext.Set<EmailAttachmentEntity>().Where(t => t.NotificationId.Equals(notification.Id)).ToListAsync();
                //var recipientsTask = DbContext.Set<NotificationEmailRecipientEntity>().Where(t => t.NotificationId.Equals(notification.Id)).ToListAsync();
                await Task.WhenAll(templatesTask, attachmentsTask);
            }

            return notifications;
        }

        public virtual Task<NotificationEntity> GetByTypeAsync(string type, string tenantId, string tenantType)
        {
            var query = Notifications;
            if (!string.IsNullOrEmpty(tenantId)) query = query.Where(q => q.TenantId.Equals(tenantId));
            if (!string.IsNullOrEmpty(tenantType)) query = query.Where(q => q.TenantType.Equals(tenantType));
            query = query.Include(n => n.Templates)
                .Include(n => n.Attachments);
                //.Include(n => n.Recipients);
            return query.FirstOrDefaultAsync(n => n.Type.Equals(type));
        }

        public NotificationEntity GetNotificationEntityForListByType(string type, string tenantId, string tenantType)
        {
            var query = Notifications;
            if (!string.IsNullOrEmpty(tenantId)) query = query.Where(q => q.TenantId.Equals(tenantId));
            if (!string.IsNullOrEmpty(tenantType)) query = query.Where(q => q.TenantType.Equals(tenantType));
            return query.FirstOrDefault(n => n.Type.Equals(type));
        }

        public Task<NotificationMessageEntity[]> GetNotificationMessageByIdAsync(string[] ids)
        {
            return NotifcationMessages.Where(x => ids.Contains(x.Id)).ToArrayAsync();
        }
    }
}
