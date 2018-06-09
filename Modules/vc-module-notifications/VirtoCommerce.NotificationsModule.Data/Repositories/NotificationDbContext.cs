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

        protected NotificationDbContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region Notification

            modelBuilder.Entity<NotificationEntity>().ToTable("Notification").HasKey(x => x.Id);
            modelBuilder.Entity<NotificationEntity>().Property(x => x.Id).HasMaxLength(128);
            
            modelBuilder.Entity<NotificationEntity>()
                .HasMany(n => n.Templates)
                .WithOne()
                .HasForeignKey(n => n.NotificationId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Cascade);
                        
            modelBuilder.Entity<NotificationEntity>().Property(x => x.CreatedBy).HasMaxLength(64);
            modelBuilder.Entity<NotificationEntity>().Property(x => x.ModifiedBy).HasMaxLength(64);

            modelBuilder.Entity<EmailNotificationEntity>()
                .HasMany(n => n.Attachments)
                .WithOne()
                .HasForeignKey(n => n.NotificationId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<EmailNotificationEntity>()
                .HasMany(n => n.Recipients)
                .WithOne()
                .HasForeignKey(n => n.NotificationId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Cascade);

            #endregion

            #region NotificationTemplate

            modelBuilder.Entity<NotificationTemplateEntity>().ToTable("NotificationTemplate").HasKey(x => x.Id);
            modelBuilder.Entity<NotificationTemplateEntity>().Property(x => x.Id).HasMaxLength(128);
            modelBuilder.Entity<NotificationTemplateEntity>().Property(x => x.CreatedBy).HasMaxLength(64);
            modelBuilder.Entity<NotificationTemplateEntity>().Property(x => x.ModifiedBy).HasMaxLength(64);

            #endregion

            #region NotificationMessage

            modelBuilder.Entity<NotificationMessageEntity>().ToTable("NotificationMessage").HasKey(x => x.Id);
            modelBuilder.Entity<NotificationMessageEntity>().Property(x => x.Id).HasMaxLength(128);
            modelBuilder.Entity<NotificationMessageEntity>().Property(x => x.CreatedBy).HasMaxLength(64);
            modelBuilder.Entity<NotificationMessageEntity>().Property(x => x.ModifiedBy).HasMaxLength(64);

            #endregion

            #region EmailAttachment

            modelBuilder.Entity<EmailAttachmentEntity>().ToTable("NotificationEmailAttachment").HasKey(x => x.Id);
            modelBuilder.Entity<EmailAttachmentEntity>().Property(x => x.Id).HasMaxLength(128);
            modelBuilder.Entity<EmailAttachmentEntity>().Property(x => x.CreatedBy).HasMaxLength(64);
            modelBuilder.Entity<EmailAttachmentEntity>().Property(x => x.ModifiedBy).HasMaxLength(64);

            #endregion

            #region NotificationEmailRecipient

            modelBuilder.Entity<NotificationEmailRecipientEntity>().ToTable("NotificationEmailRecipient").HasKey(x => x.Id);
            modelBuilder.Entity<NotificationEmailRecipientEntity>().Property(x => x.Id).HasMaxLength(128);

            #endregion

            base.OnModelCreating(modelBuilder);
        }
    }
}
