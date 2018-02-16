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
            modelBuilder.Entity<NotificationEmailRecipientEntity>().ToTable("NotificationEmailRecipient");

            modelBuilder.Entity<NotificationEntity>()
                .HasMany(n => n.CcRecipients)
                .WithOne()
                .HasForeignKey("CcRecipient_NotificationId")
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<NotificationEntity>()
                .HasMany(n => n.BccRecipients)
                .WithOne()
                .HasForeignKey("Notification_NotificationId")
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(modelBuilder);
        }
    }
}
