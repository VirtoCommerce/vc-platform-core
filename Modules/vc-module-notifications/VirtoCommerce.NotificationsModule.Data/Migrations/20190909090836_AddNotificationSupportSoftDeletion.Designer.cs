﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using VirtoCommerce.NotificationsModule.Data.Repositories;

namespace VirtoCommerce.NotificationsModule.Data.Migrations
{
    [DbContext(typeof(NotificationDbContext))]
    [Migration("20190909090836_AddNotificationSupportSoftDeletion")]
    partial class AddNotificationSupportSoftDeletion
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.3-servicing-35854")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("VirtoCommerce.NotificationsModule.Data.Model.EmailAttachmentEntity", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(128);

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(64);

                    b.Property<DateTime>("CreatedDate");

                    b.Property<string>("FileName")
                        .HasMaxLength(512);

                    b.Property<string>("LanguageCode")
                        .HasMaxLength(10);

                    b.Property<string>("MimeType")
                        .HasMaxLength(50);

                    b.Property<string>("ModifiedBy")
                        .HasMaxLength(64);

                    b.Property<DateTime?>("ModifiedDate");

                    b.Property<string>("NotificationId");

                    b.Property<string>("OuterId")
                        .HasMaxLength(128);

                    b.Property<string>("Size")
                        .HasMaxLength(128);

                    b.Property<string>("Url")
                        .HasMaxLength(2048);

                    b.HasKey("Id");

                    b.HasIndex("NotificationId");

                    b.ToTable("NotificationEmailAttachment");
                });

            modelBuilder.Entity("VirtoCommerce.NotificationsModule.Data.Model.NotificationEmailRecipientEntity", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(128);

                    b.Property<string>("EmailAddress")
                        .HasMaxLength(128);

                    b.Property<string>("NotificationId");

                    b.Property<int>("RecipientType");

                    b.HasKey("Id");

                    b.HasIndex("NotificationId");

                    b.ToTable("NotificationEmailRecipient");
                });

            modelBuilder.Entity("VirtoCommerce.NotificationsModule.Data.Model.NotificationEntity", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(128);

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(64);

                    b.Property<DateTime>("CreatedDate");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasMaxLength(128);

                    b.Property<bool>("IsActive");

                    b.Property<bool>("IsDeleted");

                    b.Property<string>("Kind")
                        .HasMaxLength(128);

                    b.Property<string>("ModifiedBy")
                        .HasMaxLength(64);

                    b.Property<DateTime?>("ModifiedDate");

                    b.Property<string>("OuterId")
                        .HasMaxLength(128);

                    b.Property<string>("TenantId")
                        .HasMaxLength(128);

                    b.Property<string>("TenantType")
                        .HasMaxLength(128);

                    b.Property<string>("Type")
                        .HasMaxLength(128);

                    b.HasKey("Id");

                    b.ToTable("Notification");

                    b.HasDiscriminator<string>("Discriminator").HasValue("NotificationEntity");
                });

            modelBuilder.Entity("VirtoCommerce.NotificationsModule.Data.Model.NotificationMessageEntity", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(128);

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(64);

                    b.Property<DateTime>("CreatedDate");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasMaxLength(128);

                    b.Property<string>("LanguageCode")
                        .HasMaxLength(10);

                    b.Property<DateTime?>("LastSendAttemptDate");

                    b.Property<string>("LastSendError");

                    b.Property<int>("MaxSendAttemptCount");

                    b.Property<string>("ModifiedBy")
                        .HasMaxLength(64);

                    b.Property<DateTime?>("ModifiedDate");

                    b.Property<string>("NotificationId");

                    b.Property<string>("NotificationType")
                        .HasMaxLength(128);

                    b.Property<string>("OuterId")
                        .HasMaxLength(128);

                    b.Property<int>("SendAttemptCount");

                    b.Property<DateTime?>("SendDate");

                    b.Property<string>("TenantId")
                        .HasMaxLength(128);

                    b.Property<string>("TenantType")
                        .HasMaxLength(128);

                    b.HasKey("Id");

                    b.HasIndex("NotificationId");

                    b.ToTable("NotificationMessage");

                    b.HasDiscriminator<string>("Discriminator").HasValue("NotificationMessageEntity");
                });

            modelBuilder.Entity("VirtoCommerce.NotificationsModule.Data.Model.NotificationTemplateEntity", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(128);

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(64);

                    b.Property<DateTime>("CreatedDate");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasMaxLength(128);

                    b.Property<string>("LanguageCode")
                        .HasMaxLength(10);

                    b.Property<string>("ModifiedBy")
                        .HasMaxLength(64);

                    b.Property<DateTime?>("ModifiedDate");

                    b.Property<string>("NotificationId")
                        .HasMaxLength(128);

                    b.Property<string>("OuterId")
                        .HasMaxLength(128);

                    b.HasKey("Id");

                    b.HasIndex("NotificationId");

                    b.ToTable("NotificationTemplate");

                    b.HasDiscriminator<string>("Discriminator").HasValue("NotificationTemplateEntity");
                });

            modelBuilder.Entity("VirtoCommerce.NotificationsModule.Data.Model.EmailNotificationEntity", b =>
                {
                    b.HasBaseType("VirtoCommerce.NotificationsModule.Data.Model.NotificationEntity");

                    b.Property<string>("From")
                        .HasMaxLength(128);

                    b.Property<string>("To")
                        .HasMaxLength(128);

                    b.HasDiscriminator().HasValue("EmailNotificationEntity");
                });

            modelBuilder.Entity("VirtoCommerce.NotificationsModule.Data.Model.SmsNotificationEntity", b =>
                {
                    b.HasBaseType("VirtoCommerce.NotificationsModule.Data.Model.NotificationEntity");

                    b.Property<string>("Number")
                        .HasMaxLength(128);

                    b.HasDiscriminator().HasValue("SmsNotificationEntity");
                });

            modelBuilder.Entity("VirtoCommerce.NotificationsModule.Data.Model.EmailNotificationMessageEntity", b =>
                {
                    b.HasBaseType("VirtoCommerce.NotificationsModule.Data.Model.NotificationMessageEntity");

                    b.Property<string>("Body");

                    b.Property<string>("Subject")
                        .HasMaxLength(512);

                    b.HasDiscriminator().HasValue("EmailNotificationMessageEntity");
                });

            modelBuilder.Entity("VirtoCommerce.NotificationsModule.Data.Model.SmsNotificationMessageEntity", b =>
                {
                    b.HasBaseType("VirtoCommerce.NotificationsModule.Data.Model.NotificationMessageEntity");

                    b.Property<string>("Message")
                        .HasMaxLength(1600);

                    b.Property<string>("Number")
                        .HasMaxLength(128);

                    b.HasDiscriminator().HasValue("SmsNotificationMessageEntity");
                });

            modelBuilder.Entity("VirtoCommerce.NotificationsModule.Data.Model.EmailNotificationTemplateEntity", b =>
                {
                    b.HasBaseType("VirtoCommerce.NotificationsModule.Data.Model.NotificationTemplateEntity");

                    b.Property<string>("Body");

                    b.Property<string>("Subject")
                        .HasMaxLength(512);

                    b.HasDiscriminator().HasValue("EmailNotificationTemplateEntity");
                });

            modelBuilder.Entity("VirtoCommerce.NotificationsModule.Data.Model.SmsNotificationTemplateEntity", b =>
                {
                    b.HasBaseType("VirtoCommerce.NotificationsModule.Data.Model.NotificationTemplateEntity");

                    b.Property<string>("Message")
                        .HasMaxLength(1600);

                    b.HasDiscriminator().HasValue("SmsNotificationTemplateEntity");
                });

            modelBuilder.Entity("VirtoCommerce.NotificationsModule.Data.Model.EmailAttachmentEntity", b =>
                {
                    b.HasOne("VirtoCommerce.NotificationsModule.Data.Model.EmailNotificationEntity", "Notification")
                        .WithMany("Attachments")
                        .HasForeignKey("NotificationId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("VirtoCommerce.NotificationsModule.Data.Model.NotificationEmailRecipientEntity", b =>
                {
                    b.HasOne("VirtoCommerce.NotificationsModule.Data.Model.EmailNotificationEntity", "Notification")
                        .WithMany("Recipients")
                        .HasForeignKey("NotificationId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("VirtoCommerce.NotificationsModule.Data.Model.NotificationMessageEntity", b =>
                {
                    b.HasOne("VirtoCommerce.NotificationsModule.Data.Model.NotificationEntity", "Notification")
                        .WithMany()
                        .HasForeignKey("NotificationId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("VirtoCommerce.NotificationsModule.Data.Model.NotificationTemplateEntity", b =>
                {
                    b.HasOne("VirtoCommerce.NotificationsModule.Data.Model.NotificationEntity", "Notification")
                        .WithMany("Templates")
                        .HasForeignKey("NotificationId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
