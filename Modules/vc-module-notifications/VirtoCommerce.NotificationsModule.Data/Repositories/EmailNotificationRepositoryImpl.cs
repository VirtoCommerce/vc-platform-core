using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VirtoCommerce.NotificationsModule.Data.Model;

namespace VirtoCommerce.NotificationsModule.Data.Repositories
{
    public class EmailNotificationRepositoryImpl : NotificationRepositoryBase, IEmailNotificationRepository
    {
        public EmailNotificationRepositoryImpl(NotificationDbContext dbContext) : base(dbContext)
        {
        }

        public IQueryable<EmailNotificationEntity> EmailNotifications => DbContext.Set<EmailNotificationEntity>();

        public override async Task<NotificationEntity> GetByTypeAsync(string type, string tenantId, string tenantType)
        {
            var notification = await base.GetByTypeAsync(type, tenantId, tenantType);

            if (notification != null)
            {
                var recipients = await DbContext.Set<NotificationEmailRecipientEntity>().Where(t => t.NotificationId.Equals(notification.Id)).ToListAsync();
            }

            return notification;
        }
    }
}
