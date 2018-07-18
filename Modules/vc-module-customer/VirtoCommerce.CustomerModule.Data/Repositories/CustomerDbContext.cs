using System;
using System.Collections.Generic;
using System.Text;
using EntityFrameworkCore.Triggers;
using Microsoft.EntityFrameworkCore;
using VirtoCommerce.CustomerModule.Data.Model;

namespace VirtoCommerce.CustomerModule.Data.Repositories
{
    public class CustomerDbContext : DbContextWithTriggers
    {
        public CustomerDbContext(DbContextOptions<CustomerDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region Member
            modelBuilder.Entity<MemberDataEntity>().HasKey(x => x.Id);
            modelBuilder.Entity<MemberDataEntity>().ToTable("Member");
            modelBuilder.Entity<MemberDataEntity>().Property(x => x.Id).HasMaxLength(128);
            modelBuilder.Entity<MemberDataEntity>().HasIndex(i => i.MemberType).IsUnique(false).HasName("IX_MemberType");
            modelBuilder.Entity<MemberDataEntity>().HasIndex(i => i.Name).IsUnique(false).HasName("IX_Member_Name");

            #endregion

            #region MemberRelation
            modelBuilder.Entity<MemberRelationDataEntity>().HasKey(x => x.Id);
            modelBuilder.Entity<MemberRelationDataEntity>().Property(x => x.Id).HasMaxLength(128);
            modelBuilder.Entity<MemberRelationDataEntity>().ToTable("MemberRelation");

            modelBuilder.Entity<MemberRelationDataEntity>().HasOne(m => m.Descendant)
                .WithMany(m => m.MemberRelations)
                .OnDelete(DeleteBehavior.Restrict).IsRequired();
            #endregion

            #region Address
            modelBuilder.Entity<AddressDataEntity>().HasKey(x => x.Id);
            modelBuilder.Entity<AddressDataEntity>().Property(x => x.Id).HasMaxLength(128);
            modelBuilder.Entity<AddressDataEntity>().ToTable("Address");

            modelBuilder.Entity<AddressDataEntity>().HasOne(m => m.Member).WithMany(m => m.Addresses)
                .HasForeignKey(m => m.MemberId).OnDelete(DeleteBehavior.Cascade).IsRequired();
            #endregion

            #region Email
            modelBuilder.Entity<EmailDataEntity>().HasKey(x => x.Id);
            modelBuilder.Entity<EmailDataEntity>().Property(x => x.Id).HasMaxLength(128);
            modelBuilder.Entity<EmailDataEntity>().ToTable("Email");

            modelBuilder.Entity<EmailDataEntity>().HasOne(m => m.Member).WithMany(m => m.Emails)
                .HasForeignKey(m => m.MemberId).OnDelete(DeleteBehavior.Cascade).IsRequired();
            modelBuilder.Entity<EmailDataEntity>().HasIndex(i => i.Address).IsUnique(false).HasName("IX_Email_Address");
            #endregion

            #region Group
            modelBuilder.Entity<MemberGroupDataEntity>().HasKey(x => x.Id);
            modelBuilder.Entity<MemberGroupDataEntity>().Property(x => x.Id).HasMaxLength(128);
            modelBuilder.Entity<MemberGroupDataEntity>().ToTable("MemberGroup");

            modelBuilder.Entity<MemberGroupDataEntity>().HasOne(m => m.Member).WithMany(m => m.Groups)
                .HasForeignKey(m => m.MemberId).OnDelete(DeleteBehavior.Cascade).IsRequired();
            modelBuilder.Entity<MemberGroupDataEntity>().HasIndex(i => i.Group).IsUnique(false).HasName("IX_MemberGroup_Group");
            #endregion

            #region Phone
            modelBuilder.Entity<PhoneDataEntity>().HasKey(x => x.Id);
            modelBuilder.Entity<PhoneDataEntity>().Property(x => x.Id).HasMaxLength(128);
            modelBuilder.Entity<PhoneDataEntity>().ToTable("Phone");

            modelBuilder.Entity<PhoneDataEntity>().HasOne(m => m.Member).WithMany(m => m.Phones)
                .HasForeignKey(m => m.MemberId).OnDelete(DeleteBehavior.Cascade).IsRequired();
            #endregion

            #region Note
            modelBuilder.Entity<NoteDataEntity>().HasKey(x => x.Id);
            modelBuilder.Entity<NoteDataEntity>().Property(x => x.Id).HasMaxLength(128);
            modelBuilder.Entity<NoteDataEntity>().ToTable("Note");

            modelBuilder.Entity<NoteDataEntity>().HasOne(m => m.Member).WithMany(m => m.Notes)
                .HasForeignKey(m => m.MemberId).OnDelete(DeleteBehavior.Restrict);
            #endregion

            #region Contact
            modelBuilder.Entity<ContactDataEntity>().Property(p => p.FirstName).HasColumnName("FirstName");
            modelBuilder.Entity<ContactDataEntity>().Property(p => p.LastName).HasColumnName("LastName");
            modelBuilder.Entity<ContactDataEntity>().Property(p => p.MiddleName).HasColumnName("MiddleName");
            modelBuilder.Entity<ContactDataEntity>().Property(p => p.FullName).HasColumnName("FullName");
            modelBuilder.Entity<ContactDataEntity>().Property(p => p.TimeZone).HasColumnName("TimeZone");
            modelBuilder.Entity<ContactDataEntity>().Property(p => p.DefaultLanguage).HasColumnName("DefaultLanguage");
            modelBuilder.Entity<ContactDataEntity>().Property(p => p.PhotoUrl).HasColumnName("PhotoUrl");
            modelBuilder.Entity<ContactDataEntity>().Property(p => p.BirthDate).HasColumnName("BirthDate");

            #endregion

            #region Organization
            modelBuilder.Entity<OrganizationDataEntity>().Property(p => p.Description).HasColumnName("Description");
            #endregion

            #region Employee
            modelBuilder.Entity<EmployeeDataEntity>().Property(p => p.FirstName).HasColumnName("FirstName");
            modelBuilder.Entity<EmployeeDataEntity>().Property(p => p.LastName).HasColumnName("LastName");
            modelBuilder.Entity<EmployeeDataEntity>().Property(p => p.MiddleName).HasColumnName("MiddleName");
            modelBuilder.Entity<EmployeeDataEntity>().Property(p => p.FullName).HasColumnName("FullName");
            modelBuilder.Entity<EmployeeDataEntity>().Property(p => p.TimeZone).HasColumnName("TimeZone");
            modelBuilder.Entity<EmployeeDataEntity>().Property(p => p.DefaultLanguage).HasColumnName("DefaultLanguage");
            modelBuilder.Entity<EmployeeDataEntity>().Property(p => p.PhotoUrl).HasColumnName("PhotoUrl");
            modelBuilder.Entity<EmployeeDataEntity>().Property(p => p.BirthDate).HasColumnName("BirthDate");

            #endregion

            #region Vendor
            modelBuilder.Entity<VendorDataEntity>().Property(p => p.Description).HasColumnName("Description");

            #endregion

            base.OnModelCreating(modelBuilder);
        }
    }
}
