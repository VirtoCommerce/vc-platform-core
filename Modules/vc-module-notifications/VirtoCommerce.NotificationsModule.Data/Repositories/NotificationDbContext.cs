using EntityFrameworkCore.Triggers;
using Microsoft.EntityFrameworkCore;
using VirtoCommerce.NotificationsModule.Data.Model;

namespace VirtoCommerce.NotificationsModule.Data.Repositories
{
    public class NotificationDbContext : DbContextWithTriggers
    {
        //Add-Migration Initial -Context VirtoCommerce.NotificationsModule.Data.Repositories.NotificationDbContext -StartupProject VirtoCommerce.NotificationsModule.Data  -Verbose -OutputDir Migrations
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
                .HasMany(n => n.Recipients)
                .WithOne()
                .HasForeignKey(n => n.NotificationId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<NotificationEntity>()
                .HasMany(n => n.Templates)
                .WithOne()
                .HasForeignKey(n => n.NotificationId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<NotificationEntity>()
                .HasMany(n => n.Attachments)
                .WithOne()
                .HasForeignKey(n => n.NotificationId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }
    }
}
