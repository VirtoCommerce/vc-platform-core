﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using VirtoCommerce.CoreModule.Data.Repositories;

namespace VirtoCommerce.CoreModule.Data.Migrations
{
    [DbContext(typeof(CoreDbContext))]
    [Migration("20180709083434_InitialCore")]
    partial class InitialCore
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.0-rtm-30799")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("VirtoCommerce.CoreModule.Data.Model.CurrencyEntity", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(128);

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasMaxLength(16);

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(64);

                    b.Property<DateTime>("CreatedDate");

                    b.Property<string>("CustomFormatting")
                        .HasMaxLength(64);

                    b.Property<decimal>("ExchangeRate")
                        .HasColumnType("Money");

                    b.Property<bool>("IsPrimary");

                    b.Property<string>("ModifiedBy")
                        .HasMaxLength(64);

                    b.Property<DateTime?>("ModifiedDate");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(256);

                    b.Property<string>("Symbol")
                        .HasMaxLength(16);

                    b.HasKey("Id");

                    b.HasIndex("Code")
                        .HasName("IX_Code");

                    b.ToTable("Currency");
                });

            modelBuilder.Entity("VirtoCommerce.CoreModule.Data.Model.PackageTypeEntity", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(128);

                    b.Property<decimal>("Height");

                    b.Property<decimal>("Length");

                    b.Property<string>("MeasureUnit")
                        .HasMaxLength(16);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(254);

                    b.Property<decimal>("Width");

                    b.HasKey("Id");

                    b.ToTable("PackageType");
                });

            modelBuilder.Entity("VirtoCommerce.CoreModule.Data.Model.SeoUrlKeywordEntity", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(128);

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(64);

                    b.Property<DateTime>("CreatedDate");

                    b.Property<string>("ImageAltDescription")
                        .HasMaxLength(255);

                    b.Property<bool>("IsActive");

                    b.Property<string>("Keyword")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<string>("Language")
                        .HasMaxLength(5);

                    b.Property<string>("MetaDescription")
                        .HasMaxLength(1024);

                    b.Property<string>("MetaKeywords")
                        .HasMaxLength(255);

                    b.Property<string>("ModifiedBy")
                        .HasMaxLength(64);

                    b.Property<DateTime?>("ModifiedDate");

                    b.Property<string>("ObjectId")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<string>("ObjectType")
                        .IsRequired()
                        .HasMaxLength(64);

                    b.Property<string>("StoreId")
                        .HasMaxLength(128);

                    b.Property<string>("Title")
                        .HasMaxLength(255);

                    b.HasKey("Id");

                    b.HasIndex("Keyword", "StoreId")
                        .HasName("IX_KeywordStoreId");

                    b.HasIndex("ObjectId", "ObjectType")
                        .HasName("IX_ObjectIdAndObjectType");

                    b.ToTable("SeoUrlKeyword");
                });

            modelBuilder.Entity("VirtoCommerce.CoreModule.Data.Model.SequenceEntity", b =>
                {
                    b.Property<string>("ObjectType")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(256);

                    b.Property<DateTime?>("ModifiedDate");

                    b.Property<int>("Value");

                    b.HasKey("ObjectType");

                    b.ToTable("Sequence");
                });
#pragma warning restore 612, 618
        }
    }
}
