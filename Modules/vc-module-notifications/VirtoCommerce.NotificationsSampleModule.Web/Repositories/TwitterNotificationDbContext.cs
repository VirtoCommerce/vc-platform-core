using Microsoft.EntityFrameworkCore;
using VirtoCommerce.NotificationsModule.Data.Repositories;
using VirtoCommerce.NotificationsSampleModule.Web.Models;
using VirtoCommerce.Platform.Data.Repositories;

namespace VirtoCommerce.NotificationsSampleModule.Web.Repositories
{
    public class TwitterNotificationDbContext : NotificationDbContext
    {
        public TwitterNotificationDbContext(DbContextOptions<TwitterNotificationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TwitterNotificationEntity>(entity =>
            {
                entity.ToTable("Notification");
            });
        }
    }
}
