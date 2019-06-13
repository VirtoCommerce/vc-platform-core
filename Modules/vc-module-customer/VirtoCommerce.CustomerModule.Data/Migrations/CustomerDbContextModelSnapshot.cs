﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using VirtoCommerce.CustomerModule.Data.Repositories;

namespace VirtoCommerce.CustomerModule.Data.Migrations
{
    [DbContext(typeof(CustomerDbContext))]
    partial class CustomerDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.3-servicing-35854")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("VirtoCommerce.CustomerModule.Data.Model.AddressEntity", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(128);

                    b.Property<string>("City")
                        .IsRequired()
                        .HasMaxLength(128);

                    b.Property<string>("CountryCode")
                        .IsRequired()
                        .HasMaxLength(64);

                    b.Property<string>("CountryName")
                        .IsRequired()
                        .HasMaxLength(128);

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(64);

                    b.Property<DateTime>("CreatedDate");

                    b.Property<string>("DaytimePhoneNumber")
                        .HasMaxLength(64);

                    b.Property<string>("Email")
                        .HasMaxLength(256);

                    b.Property<string>("EveningPhoneNumber")
                        .HasMaxLength(64);

                    b.Property<string>("FaxNumber")
                        .HasMaxLength(64);

                    b.Property<string>("FirstName")
                        .HasMaxLength(128);

                    b.Property<string>("LastName")
                        .HasMaxLength(128);

                    b.Property<string>("Line1")
                        .IsRequired()
                        .HasMaxLength(128);

                    b.Property<string>("Line2")
                        .HasMaxLength(128);

                    b.Property<string>("MemberId")
                        .IsRequired();

                    b.Property<string>("ModifiedBy")
                        .HasMaxLength(64);

                    b.Property<DateTime?>("ModifiedDate");

                    b.Property<string>("Name")
                        .HasMaxLength(2048);

                    b.Property<string>("Organization")
                        .HasMaxLength(128);

                    b.Property<string>("OuterId")
                        .HasMaxLength(128);

                    b.Property<string>("PostalCode")
                        .IsRequired()
                        .HasMaxLength(32);

                    b.Property<string>("RegionId")
                        .HasMaxLength(128);

                    b.Property<string>("RegionName")
                        .HasMaxLength(128);

                    b.Property<string>("StateProvince")
                        .HasMaxLength(128);

                    b.Property<string>("Type")
                        .HasMaxLength(64);

                    b.HasKey("Id");

                    b.HasIndex("MemberId");

                    b.ToTable("Address");
                });

            modelBuilder.Entity("VirtoCommerce.CustomerModule.Data.Model.EmailEntity", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(128);

                    b.Property<string>("Address")
                        .HasMaxLength(254);

                    b.Property<bool>("IsValidated");

                    b.Property<string>("MemberId")
                        .IsRequired();

                    b.Property<string>("Type")
                        .HasMaxLength(64);

                    b.HasKey("Id");

                    b.HasIndex("Address")
                        .HasName("IX_Email_Address");

                    b.HasIndex("MemberId");

                    b.ToTable("Email");
                });

            modelBuilder.Entity("VirtoCommerce.CustomerModule.Data.Model.MemberEntity", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(128);

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(64);

                    b.Property<DateTime>("CreatedDate");

                    b.Property<string>("Discriminator")
                        .IsRequired();

                    b.Property<string>("MemberType")
                        .HasMaxLength(64);

                    b.Property<string>("ModifiedBy")
                        .HasMaxLength(64);

                    b.Property<DateTime?>("ModifiedDate");

                    b.Property<string>("Name")
                        .HasMaxLength(128);

                    b.Property<string>("OuterId")
                        .HasMaxLength(128);

                    b.HasKey("Id");

                    b.HasIndex("MemberType")
                        .HasName("IX_MemberType");

                    b.HasIndex("Name")
                        .HasName("IX_Member_Name");

                    b.ToTable("Member");

                    b.HasDiscriminator<string>("Discriminator").HasValue("MemberEntity");
                });

            modelBuilder.Entity("VirtoCommerce.CustomerModule.Data.Model.MemberGroupEntity", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(128);

                    b.Property<string>("Group")
                        .HasMaxLength(64);

                    b.Property<string>("MemberId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("Group")
                        .HasName("IX_MemberGroup_Group");

                    b.HasIndex("MemberId");

                    b.ToTable("MemberGroup");
                });

            modelBuilder.Entity("VirtoCommerce.CustomerModule.Data.Model.MemberRelationEntity", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(128);

                    b.Property<string>("AncestorId")
                        .IsRequired()
                        .HasMaxLength(128);

                    b.Property<int>("AncestorSequence");

                    b.Property<string>("DescendantId")
                        .IsRequired()
                        .HasMaxLength(128);

                    b.Property<string>("RelationType")
                        .HasMaxLength(64);

                    b.HasKey("Id");

                    b.HasIndex("AncestorId");

                    b.HasIndex("DescendantId");

                    b.ToTable("MemberRelation");
                });

            modelBuilder.Entity("VirtoCommerce.CustomerModule.Data.Model.NoteEntity", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(128);

                    b.Property<string>("AuthorName")
                        .HasMaxLength(128);

                    b.Property<string>("Body");

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(64);

                    b.Property<DateTime>("CreatedDate");

                    b.Property<bool>("IsSticky");

                    b.Property<string>("MemberId");

                    b.Property<string>("ModifiedBy")
                        .HasMaxLength(64);

                    b.Property<DateTime?>("ModifiedDate");

                    b.Property<string>("ModifierName")
                        .HasMaxLength(128);

                    b.Property<string>("OuterId")
                        .HasMaxLength(128);

                    b.Property<string>("Title")
                        .HasMaxLength(128);

                    b.HasKey("Id");

                    b.HasIndex("MemberId");

                    b.ToTable("Note");
                });

            modelBuilder.Entity("VirtoCommerce.CustomerModule.Data.Model.PhoneEntity", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(128);

                    b.Property<string>("MemberId")
                        .IsRequired();

                    b.Property<string>("Number")
                        .HasMaxLength(64);

                    b.Property<string>("Type")
                        .HasMaxLength(64);

                    b.HasKey("Id");

                    b.HasIndex("MemberId");

                    b.ToTable("Phone");
                });

            modelBuilder.Entity("VirtoCommerce.CustomerModule.Data.Model.SeoInfoEntity", b =>
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

                    b.Property<string>("MemberId");

                    b.Property<string>("MetaDescription")
                        .HasMaxLength(1024);

                    b.Property<string>("MetaKeywords")
                        .HasMaxLength(255);

                    b.Property<string>("ModifiedBy")
                        .HasMaxLength(64);

                    b.Property<DateTime?>("ModifiedDate");

                    b.Property<string>("OuterId")
                        .HasMaxLength(128);

                    b.Property<string>("StoreId")
                        .HasMaxLength(128);

                    b.Property<string>("Title")
                        .HasMaxLength(255);

                    b.HasKey("Id");

                    b.HasIndex("MemberId");

                    b.ToTable("MemberSeoInfo");
                });

            modelBuilder.Entity("VirtoCommerce.CustomerModule.Data.Model.ContactEntity", b =>
                {
                    b.HasBaseType("VirtoCommerce.CustomerModule.Data.Model.MemberEntity");

                    b.Property<DateTime?>("BirthDate")
                        .HasColumnName("BirthDate");

                    b.Property<string>("DefaultLanguage")
                        .HasColumnName("DefaultLanguage")
                        .HasMaxLength(32);

                    b.Property<string>("FirstName")
                        .HasColumnName("FirstName")
                        .HasMaxLength(128);

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasColumnName("FullName")
                        .HasMaxLength(254);

                    b.Property<string>("LastName")
                        .HasColumnName("LastName")
                        .HasMaxLength(128);

                    b.Property<string>("MiddleName")
                        .HasColumnName("MiddleName")
                        .HasMaxLength(128);

                    b.Property<string>("PhotoUrl")
                        .HasColumnName("PhotoUrl")
                        .HasMaxLength(2083);

                    b.Property<string>("PreferredCommunication")
                        .HasMaxLength(64);

                    b.Property<string>("PreferredDelivery")
                        .HasMaxLength(64);

                    b.Property<string>("Salutation")
                        .HasMaxLength(256);

                    b.Property<string>("TaxpayerId")
                        .HasMaxLength(64);

                    b.Property<string>("TimeZone")
                        .HasColumnName("TimeZone")
                        .HasMaxLength(32);

                    b.HasDiscriminator().HasValue("ContactEntity");
                });

            modelBuilder.Entity("VirtoCommerce.CustomerModule.Data.Model.EmployeeEntity", b =>
                {
                    b.HasBaseType("VirtoCommerce.CustomerModule.Data.Model.MemberEntity");

                    b.Property<DateTime?>("BirthDate")
                        .HasColumnName("BirthDate");

                    b.Property<string>("DefaultLanguage")
                        .HasColumnName("DefaultLanguage")
                        .HasMaxLength(32);

                    b.Property<string>("FirstName")
                        .HasColumnName("FirstName")
                        .HasMaxLength(128);

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasColumnName("FullName")
                        .HasMaxLength(254);

                    b.Property<bool>("IsActive");

                    b.Property<string>("LastName")
                        .HasColumnName("LastName")
                        .HasMaxLength(128);

                    b.Property<string>("MiddleName")
                        .HasColumnName("MiddleName")
                        .HasMaxLength(128);

                    b.Property<string>("PhotoUrl")
                        .HasColumnName("PhotoUrl")
                        .HasMaxLength(2083);

                    b.Property<string>("TimeZone")
                        .HasColumnName("TimeZone")
                        .HasMaxLength(32);

                    b.Property<string>("Type")
                        .HasMaxLength(64);

                    b.HasDiscriminator().HasValue("EmployeeEntity");
                });

            modelBuilder.Entity("VirtoCommerce.CustomerModule.Data.Model.OrganizationEntity", b =>
                {
                    b.HasBaseType("VirtoCommerce.CustomerModule.Data.Model.MemberEntity");

                    b.Property<string>("BusinessCategory")
                        .HasMaxLength(64);

                    b.Property<string>("Description")
                        .HasColumnName("Description")
                        .HasMaxLength(256);

                    b.Property<string>("OwnerId")
                        .HasMaxLength(128);

                    b.HasDiscriminator().HasValue("OrganizationEntity");
                });

            modelBuilder.Entity("VirtoCommerce.CustomerModule.Data.Model.VendorEntity", b =>
                {
                    b.HasBaseType("VirtoCommerce.CustomerModule.Data.Model.MemberEntity");

                    b.Property<string>("Description")
                        .HasColumnName("Description")
                        .HasMaxLength(256);

                    b.Property<string>("GroupName")
                        .HasMaxLength(64);

                    b.Property<string>("LogoUrl")
                        .HasMaxLength(2048);

                    b.Property<string>("SiteUrl")
                        .HasMaxLength(2048);

                    b.HasDiscriminator().HasValue("VendorEntity");
                });

            modelBuilder.Entity("VirtoCommerce.CustomerModule.Data.Model.AddressEntity", b =>
                {
                    b.HasOne("VirtoCommerce.CustomerModule.Data.Model.MemberEntity", "Member")
                        .WithMany("Addresses")
                        .HasForeignKey("MemberId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("VirtoCommerce.CustomerModule.Data.Model.EmailEntity", b =>
                {
                    b.HasOne("VirtoCommerce.CustomerModule.Data.Model.MemberEntity", "Member")
                        .WithMany("Emails")
                        .HasForeignKey("MemberId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("VirtoCommerce.CustomerModule.Data.Model.MemberGroupEntity", b =>
                {
                    b.HasOne("VirtoCommerce.CustomerModule.Data.Model.MemberEntity", "Member")
                        .WithMany("Groups")
                        .HasForeignKey("MemberId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("VirtoCommerce.CustomerModule.Data.Model.MemberRelationEntity", b =>
                {
                    b.HasOne("VirtoCommerce.CustomerModule.Data.Model.MemberEntity", "Ancestor")
                        .WithMany()
                        .HasForeignKey("AncestorId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("VirtoCommerce.CustomerModule.Data.Model.MemberEntity", "Descendant")
                        .WithMany("MemberRelations")
                        .HasForeignKey("DescendantId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("VirtoCommerce.CustomerModule.Data.Model.NoteEntity", b =>
                {
                    b.HasOne("VirtoCommerce.CustomerModule.Data.Model.MemberEntity", "Member")
                        .WithMany("Notes")
                        .HasForeignKey("MemberId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("VirtoCommerce.CustomerModule.Data.Model.PhoneEntity", b =>
                {
                    b.HasOne("VirtoCommerce.CustomerModule.Data.Model.MemberEntity", "Member")
                        .WithMany("Phones")
                        .HasForeignKey("MemberId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("VirtoCommerce.CustomerModule.Data.Model.SeoInfoEntity", b =>
                {
                    b.HasOne("VirtoCommerce.CustomerModule.Data.Model.MemberEntity", "Member")
                        .WithMany("SeoInfos")
                        .HasForeignKey("MemberId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
