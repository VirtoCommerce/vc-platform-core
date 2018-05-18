using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VirtoCommerce.NotificationsModule.Data.Model;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Data.Infrastructure;

namespace VirtoCommerce.NotificationsModule.Data.Repositories
{
    public class NotificationRepository : DbContextRepositoryBase<NotificationDbContext>, INotificationRepository
    {
        public NotificationRepository(NotificationDbContext dbContext) : base(dbContext)
        {
        }

        public IQueryable<EmailNotificationEntity> EmailNotifications => DbContext.Set<EmailNotificationEntity>();
        public IQueryable<SmsNotificationEntity> SmsNotifications => DbContext.Set<SmsNotificationEntity>();
        public IQueryable<NotificationEntity> Notifications => DbContext.Set<NotificationEntity>();
        public IQueryable<NotificationMessageEntity> NotifcationMessages => DbContext.Set<NotificationMessageEntity>();

        public async Task<NotificationEntity[]> GetByIdsAsync(string[] ids, string responseGroup)
        {
            var notifications = await Notifications
                .Where(x => ids.Contains(x.Id))
                .OrderBy(x => x.Type)
                .ToArrayAsync();

            var notificaionResponseGroup = EnumUtility.SafeParse(responseGroup, NotificationResponseGroup.Full);

            foreach (var notification in notifications)
            {
                if ((notificaionResponseGroup & NotificationResponseGroup.WithTemplates) == NotificationResponseGroup.WithTemplates)
                {
                    var templates = await DbContext.Set<NotificationTemplateEntity>().Where(t => t.NotificationId.Equals(notification.Id)).ToListAsync();
                }

                if ((notificaionResponseGroup & NotificationResponseGroup.WithAttachments) == NotificationResponseGroup.WithAttachments)
                {
                    var attachments = await DbContext.Set<EmailAttachmentEntity>().Where(t => t.NotificationId.Equals(notification.Id)).ToListAsync();
                }

                if ((notificaionResponseGroup & NotificationResponseGroup.WithRecipients) == NotificationResponseGroup.WithRecipients)
                {
                    var recipients = await DbContext.Set<NotificationEmailRecipientEntity>().Where(t => t.NotificationId.Equals(notification.Id)).ToListAsync();
                }
            }

            return notifications;
        }

        public virtual async Task<NotificationEntity> GetByTypeAsync(string type, string tenantId, string tenantType, string responseGroup)
        {
            var query = Notifications;

            if (!string.IsNullOrEmpty(tenantId))
            {
                query = query.Where(q => q.TenantId.Equals(tenantId));
            }

            if (!string.IsNullOrEmpty(tenantType))
            {
                query = query.Where(q => q.TenantType.Equals(tenantType));
            }

            query = query.Include(n => n.Templates);

            var result = await query.FirstOrDefaultAsync(n => n.Type.Equals(type));

            if (result != null)
            {
                var notificaionResponseGroup = EnumUtility.SafeParse(responseGroup, NotificationResponseGroup.Full);

                if ((notificaionResponseGroup & NotificationResponseGroup.WithTemplates) == NotificationResponseGroup.WithTemplates)
                {
                    var templates = await DbContext.Set<NotificationTemplateEntity>().Where(t => t.NotificationId.Equals(result.Id)).ToListAsync();
                }

                if ((notificaionResponseGroup & NotificationResponseGroup.WithAttachments) == NotificationResponseGroup.WithAttachments)
                {
                    var attachments = await DbContext.Set<EmailAttachmentEntity>().Where(t => t.NotificationId.Equals(result.Id)).ToListAsync();
                }

                if ((notificaionResponseGroup & NotificationResponseGroup.WithRecipients) == NotificationResponseGroup.WithRecipients)
                {
                    var recipients = await DbContext.Set<NotificationEmailRecipientEntity>().Where(t => t.NotificationId.Equals(result.Id)).ToListAsync();
                }
            }

            return result;
        }

        public Task<NotificationMessageEntity[]> GetMessageByIdAsync(string[] ids)
        {
            return NotifcationMessages.Where(x => ids.Contains(x.Id)).ToArrayAsync();
        }
    }
}
