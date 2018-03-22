using System.Linq;
using VirtoCommerce.NotificationsModule.Data.Model;

namespace VirtoCommerce.NotificationsModule.Data.Repositories
{
    public interface IEmailNotificationRepository : INotificationRepository
    {
        IQueryable<EmailNotificationEntity> EmailNotifications { get; }
    }
}
