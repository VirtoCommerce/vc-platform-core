﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using VirtoCommerce.SitemapsModule.Data.Repositories;

namespace VirtoCommerce.SitemapsModule.Data.Migrations
{
    [DbContext(typeof(SitemapDbContext))]
    [Migration("20190909123559_AddSitemapsSupportSoftDeletion")]
    partial class AddSitemapsSupportSoftDeletion
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.3-servicing-35854")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("VirtoCommerce.SitemapsModule.Data.Models.SitemapEntity", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(128);

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(64);

                    b.Property<DateTime>("CreatedDate");

                    b.Property<string>("Filename")
                        .IsRequired()
                        .HasMaxLength(256);

                    b.Property<bool>("IsDeleted");

                    b.Property<string>("ModifiedBy")
                        .HasMaxLength(64);

                    b.Property<DateTime?>("ModifiedDate");

                    b.Property<string>("StoreId")
                        .IsRequired()
                        .HasMaxLength(64);

                    b.Property<string>("UrlTemplate")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("Filename");

                    b.ToTable("Sitemap");
                });

            modelBuilder.Entity("VirtoCommerce.SitemapsModule.Data.Models.SitemapItemEntity", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(128);

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(64);

                    b.Property<DateTime>("CreatedDate");

                    b.Property<string>("ImageUrl")
                        .HasMaxLength(512);

                    b.Property<bool>("IsDeleted");

                    b.Property<string>("ModifiedBy")
                        .HasMaxLength(64);

                    b.Property<DateTime?>("ModifiedDate");

                    b.Property<string>("ObjectId")
                        .HasMaxLength(128);

                    b.Property<string>("ObjectType")
                        .IsRequired()
                        .HasMaxLength(128);

                    b.Property<string>("SitemapId")
                        .IsRequired();

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(256);

                    b.Property<string>("UrlTemplate")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("SitemapId");

                    b.ToTable("SitemapItem");
                });

            modelBuilder.Entity("VirtoCommerce.SitemapsModule.Data.Models.SitemapItemEntity", b =>
                {
                    b.HasOne("VirtoCommerce.SitemapsModule.Data.Models.SitemapEntity", "Sitemap")
                        .WithMany("Items")
                        .HasForeignKey("SitemapId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
