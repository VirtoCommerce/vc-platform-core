﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using VirtoCommerce.TaxModule.Data.Repositories;

namespace VirtoCommerce.TaxModule.Data.Migrations
{
    [DbContext(typeof(TaxDbContext))]
    [Migration("20190909130019_AddTaxSupportSoftDeletion")]
    partial class AddTaxSupportSoftDeletion
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.3-servicing-35854")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("VirtoCommerce.TaxModule.Data.Model.StoreTaxProviderEntity", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(128);

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasMaxLength(128);

                    b.Property<bool>("IsActive");

                    b.Property<bool>("IsDeleted");

                    b.Property<string>("LogoUrl")
                        .HasMaxLength(2048);

                    b.Property<int>("Priority");

                    b.Property<string>("StoreId")
                        .HasMaxLength(128);

                    b.Property<string>("TypeName")
                        .IsRequired()
                        .HasMaxLength(128);

                    b.HasKey("Id");

                    b.HasIndex("TypeName", "StoreId")
                        .IsUnique()
                        .HasName("IX_StoreTaxProviderEntity_TypeName_StoreId")
                        .HasFilter("[StoreId] IS NOT NULL");

                    b.ToTable("StoreTaxProvider");
                });
#pragma warning restore 612, 618
        }
    }
}
