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

            #endregion

            #region MemberRelation
            modelBuilder.Entity<MemberRelationDataEntity>().HasKey(x => x.Id);
            modelBuilder.Entity<MemberRelationDataEntity>().ToTable("MemberRelation");

            modelBuilder.Entity<MemberRelationDataEntity>().HasOne(m => m.Descendant)
                .WithMany(m => m.MemberRelations)
                .OnDelete(DeleteBehavior.Cascade).IsRequired();
            #endregion

            #region Address
            modelBuilder.Entity<AddressDataEntity>().HasKey(x => x.Id);
            modelBuilder.Entity<AddressDataEntity>().ToTable("Address");

            modelBuilder.Entity<AddressDataEntity>().HasOne(m => m.Member).WithMany(m => m.Addresses)
                .HasForeignKey(m => m.MemberId).OnDelete(DeleteBehavior.Cascade).IsRequired();
            #endregion

            #region Email
            modelBuilder.Entity<EmailDataEntity>().HasKey(x => x.Id);
            modelBuilder.Entity<EmailDataEntity>().ToTable("Email");

            modelBuilder.Entity<EmailDataEntity>().HasOne(m => m.Member).WithMany(m => m.Emails)
                .HasForeignKey(m => m.MemberId).OnDelete(DeleteBehavior.Cascade).IsRequired();
            #endregion

            #region Group
            modelBuilder.Entity<MemberGroupDataEntity>().HasKey(x => x.Id);
            modelBuilder.Entity<MemberGroupDataEntity>().ToTable("MemberGroup");

            modelBuilder.Entity<MemberGroupDataEntity>().HasOne(m => m.Member).WithMany(m => m.Groups)
                .HasForeignKey(m => m.MemberId).OnDelete(DeleteBehavior.Cascade).IsRequired();
            #endregion

            #region Phone
            modelBuilder.Entity<PhoneDataEntity>().HasKey(x => x.Id);
            modelBuilder.Entity<PhoneDataEntity>().ToTable("Phone");

            modelBuilder.Entity<PhoneDataEntity>().HasOne(m => m.Member).WithMany(m => m.Phones)
                .HasForeignKey(m => m.MemberId).OnDelete(DeleteBehavior.Cascade).IsRequired();
            #endregion

            #region Note
            modelBuilder.Entity<NoteDataEntity>().HasKey(x => x.Id);
            modelBuilder.Entity<NoteDataEntity>().ToTable("Note");

            modelBuilder.Entity<NoteDataEntity>().HasOne(m => m.Member).WithMany(m => m.Notes)
                .HasForeignKey(m => m.MemberId).OnDelete(DeleteBehavior.Restrict);
            #endregion

            #region Contact
            modelBuilder.Entity<ContactDataEntity>().HasKey(x => x.Id);
            modelBuilder.Entity<ContactDataEntity>().ToTable("Contact");

            #endregion

            #region Organization
            modelBuilder.Entity<OrganizationDataEntity>().HasKey(x => x.Id);
            modelBuilder.Entity<OrganizationDataEntity>().ToTable("Organization");
            #endregion

            #region Employee
            modelBuilder.Entity<EmployeeDataEntity>().HasKey(x => x.Id);
            modelBuilder.Entity<EmployeeDataEntity>().ToTable("Employee");

            #endregion

            #region Vendor
            modelBuilder.Entity<VendorDataEntity>().HasKey(x => x.Id);
            modelBuilder.Entity<VendorDataEntity>().ToTable("Vendor");

            #endregion

            base.OnModelCreating(modelBuilder);
        }
    }
}
