using Microsoft.EntityFrameworkCore;
using VirtoCommerce.NotificationsModule.Data.Model;
using VirtoCommerce.Platform.Data.Infrastructure;

namespace VirtoCommerce.NotificationsModule.Data.Repositories
{
    public class NotificationDbContext : DbContextWithTriggersAndQueryFiltersBase
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
            modelBuilder.Entity<NotificationEntity>().Property(x => x.CreatedBy).HasMaxLength(64);
            modelBuilder.Entity<NotificationEntity>().Property(x => x.ModifiedBy).HasMaxLength(64);
            modelBuilder.Entity<NotificationEntity>().HasDiscriminator<string>("Discriminator");
            modelBuilder.Entity<NotificationEntity>().Property("Discriminator").HasMaxLength(128);

            modelBuilder.Entity<EmailNotificationEntity>();
            modelBuilder.Entity<SmsNotificationEntity>();

            #endregion

            #region NotificationTemplate

            modelBuilder.Entity<NotificationTemplateEntity>().ToTable("NotificationTemplate").HasKey(x => x.Id);
            modelBuilder.Entity<NotificationTemplateEntity>().Property(x => x.Id).HasMaxLength(128);
            modelBuilder.Entity<NotificationTemplateEntity>().Property(x => x.CreatedBy).HasMaxLength(64);
            modelBuilder.Entity<NotificationTemplateEntity>().Property(x => x.ModifiedBy).HasMaxLength(64);
            modelBuilder.Entity<NotificationTemplateEntity>().HasDiscriminator<string>("Discriminator");
            modelBuilder.Entity<NotificationTemplateEntity>().Property("Discriminator").HasMaxLength(128);

            modelBuilder.Entity<NotificationTemplateEntity>().HasOne(x => x.Notification).WithMany(x => x.Templates)
                       .HasForeignKey(x => x.NotificationId).OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<EmailNotificationTemplateEntity>();
            modelBuilder.Entity<SmsNotificationTemplateEntity>();

            #endregion

            #region NotificationMessage

            modelBuilder.Entity<NotificationMessageEntity>().ToTable("NotificationMessage").HasKey(x => x.Id);
            modelBuilder.Entity<NotificationMessageEntity>().Property(x => x.Id).HasMaxLength(128);
            modelBuilder.Entity<NotificationMessageEntity>().Property(x => x.CreatedBy).HasMaxLength(64);
            modelBuilder.Entity<NotificationMessageEntity>().Property(x => x.ModifiedBy).HasMaxLength(64);
            modelBuilder.Entity<NotificationMessageEntity>().HasDiscriminator<string>("Discriminator");
            modelBuilder.Entity<NotificationMessageEntity>().Property("Discriminator").HasMaxLength(128);

            modelBuilder.Entity<NotificationMessageEntity>().HasOne(x => x.Notification).WithMany()
                  .HasForeignKey(x => x.NotificationId).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EmailNotificationMessageEntity>();
            modelBuilder.Entity<SmsNotificationMessageEntity>();

            #endregion

            #region EmailAttachment

            modelBuilder.Entity<EmailAttachmentEntity>().ToTable("NotificationEmailAttachment").HasKey(x => x.Id);
            modelBuilder.Entity<EmailAttachmentEntity>().Property(x => x.Id).HasMaxLength(128);
            modelBuilder.Entity<EmailAttachmentEntity>().Property(x => x.CreatedBy).HasMaxLength(64);
            modelBuilder.Entity<EmailAttachmentEntity>().Property(x => x.ModifiedBy).HasMaxLength(64);

            modelBuilder.Entity<EmailAttachmentEntity>().HasOne(x => x.Notification).WithMany(x => x.Attachments)
                  .HasForeignKey(x => x.NotificationId).OnDelete(DeleteBehavior.Cascade);

            #endregion

            #region NotificationEmailRecipient

            modelBuilder.Entity<NotificationEmailRecipientEntity>().ToTable("NotificationEmailRecipient").HasKey(x => x.Id);
            modelBuilder.Entity<NotificationEmailRecipientEntity>().Property(x => x.Id).HasMaxLength(128);

            modelBuilder.Entity<NotificationEmailRecipientEntity>().HasOne(x => x.Notification).WithMany(x => x.Recipients)
                  .HasForeignKey(x => x.NotificationId).OnDelete(DeleteBehavior.Cascade);

            #endregion

            base.OnModelCreating(modelBuilder);
        }
    }
}
