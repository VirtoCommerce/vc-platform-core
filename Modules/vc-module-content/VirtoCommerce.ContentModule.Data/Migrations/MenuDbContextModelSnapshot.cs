﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using VirtoCommerce.ContentModule.Data.Repositories;

namespace VirtoCommerce.ContentModule.Data.Migrations
{
    [DbContext(typeof(MenuDbContext))]
    partial class MenuDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.3-servicing-35854")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("VirtoCommerce.ContentModule.Data.Model.MenuLinkEntity", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(128);

                    b.Property<string>("AssociatedObjectId")
                        .HasMaxLength(128);

                    b.Property<string>("AssociatedObjectName")
                        .HasMaxLength(254);

                    b.Property<string>("AssociatedObjectType")
                        .HasMaxLength(254);

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(64);

                    b.Property<DateTime>("CreatedDate");

                    b.Property<string>("MenuLinkListId");

                    b.Property<string>("ModifiedBy")
                        .HasMaxLength(64);

                    b.Property<DateTime?>("ModifiedDate");

                    b.Property<int>("Priority");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(1024);

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasMaxLength(2048);

                    b.HasKey("Id");

                    b.HasIndex("MenuLinkListId");

                    b.ToTable("ContentMenuLink");
                });

            modelBuilder.Entity("VirtoCommerce.ContentModule.Data.Model.MenuLinkListEntity", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(128);

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(64);

                    b.Property<DateTime>("CreatedDate");

                    b.Property<string>("Language");

                    b.Property<string>("ModifiedBy")
                        .HasMaxLength(64);

                    b.Property<DateTime?>("ModifiedDate");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<string>("StoreId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("ContentMenuLinkList");
                });

            modelBuilder.Entity("VirtoCommerce.ContentModule.Data.Model.MenuLinkEntity", b =>
                {
                    b.HasOne("VirtoCommerce.ContentModule.Data.Model.MenuLinkListEntity", "MenuLinkList")
                        .WithMany("MenuLinks")
                        .HasForeignKey("MenuLinkListId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
