using System.Linq;
using VirtoCommerce.NotificationsModule.Data.Repositories;
using VirtoCommerce.NotificationsSampleModule.Web.Models;

namespace VirtoCommerce.NotificationsSampleModule.Web.Repositories
{
    public class TwitterNotificationRepository : NotificationRepository
    {
        public TwitterNotificationRepository(TwitterNotificationDbContext dbContext) : base(dbContext)
        {
        }

        public IQueryable<TwitterNotificationEntity> TwitterNotifications => DbContext.Set<TwitterNotificationEntity>();
    }
}
