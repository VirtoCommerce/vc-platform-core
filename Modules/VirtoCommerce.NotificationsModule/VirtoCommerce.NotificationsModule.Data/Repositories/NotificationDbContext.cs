using EntityFrameworkCore.Triggers;
using Microsoft.EntityFrameworkCore;
using VirtoCommerce.NotificationsModule.Data.Model;

namespace VirtoCommerce.NotificationsModule.Data.Repositories
{
    public class NotificationDbContext : DbContextWithTriggers
    {
        public NotificationDbContext(DbContextOptions<NotificationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<NotificationEntity>().ToTable("Notification");
            modelBuilder.Entity<NotificationTemplateEntity>().ToTable("NotificationTemplate");
            modelBuilder.Entity<NotificationMessageEntity>().ToTable("NotificationMessage");
            modelBuilder.Entity<EmailAttachmentEntity>().ToTable("NotificationEmailAttachment");

            base.OnModelCreating(modelBuilder);
        }
    }
}
