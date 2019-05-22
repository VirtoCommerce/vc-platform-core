using System;
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
            if (ids == null)
            {
                throw new ArgumentNullException(nameof(ids));
            }

            var result = Array.Empty<NotificationEntity>();
            if (ids.Any())
            {
                result = await Notifications.Where(x => ids.Contains(x.Id)).OrderBy(x => x.Type).ToArrayAsync();
                ids = result.Select(x => x.Id).ToArray();

                var notificaionResponseGroup = EnumUtility.SafeParseFlags(responseGroup, NotificationResponseGroup.Full);

                if (ids.Any())
                {
                    if (notificaionResponseGroup.HasFlag(NotificationResponseGroup.WithTemplates))
                    {
                        var templates = await DbContext.Set<NotificationTemplateEntity>().Where(t => ids.Contains(t.NotificationId)).ToListAsync();
                    }
                    if (notificaionResponseGroup.HasFlag(NotificationResponseGroup.WithAttachments))
                    {
                        var attachments = await DbContext.Set<EmailAttachmentEntity>().Where(t => ids.Contains(t.NotificationId)).ToListAsync();
                    }
                    if (notificaionResponseGroup.HasFlag(NotificationResponseGroup.WithRecipients))
                    {
                        var recipients = await DbContext.Set<NotificationEmailRecipientEntity>().Where(t => ids.Contains(t.NotificationId)).ToListAsync();
                    }
                }
            }
            return result;
        }



        public async Task<NotificationMessageEntity[]> GetMessagesByIdsAsync(string[] ids)
        {
            return await NotifcationMessages.Where(x => ids.Contains(x.Id)).ToArrayAsync();
        }
    }
}
