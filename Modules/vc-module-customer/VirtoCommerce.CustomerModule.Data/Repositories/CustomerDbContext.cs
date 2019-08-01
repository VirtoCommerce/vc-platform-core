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

        protected CustomerDbContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region Member

            modelBuilder.Entity<MemberEntity>().HasKey(x => x.Id);
            modelBuilder.Entity<MemberEntity>().ToTable("Member");
            modelBuilder.Entity<MemberEntity>().Property(x => x.Id).HasMaxLength(128);
            modelBuilder.Entity<MemberEntity>().HasIndex(i => i.MemberType).IsUnique(false).HasName("IX_MemberType");
            modelBuilder.Entity<MemberEntity>().HasIndex(i => i.Name).IsUnique(false).HasName("IX_Member_Name");
            modelBuilder.Entity<MemberEntity>().HasDiscriminator<string>("Discriminator");
            modelBuilder.Entity<MemberEntity>().Property("Discriminator").HasMaxLength(128);

            #endregion

            #region MemberRelation
            modelBuilder.Entity<MemberRelationEntity>().HasKey(x => x.Id);
            modelBuilder.Entity<MemberRelationEntity>().Property(x => x.Id).HasMaxLength(128);
            modelBuilder.Entity<MemberRelationEntity>().ToTable("MemberRelation");

            modelBuilder.Entity<MemberRelationEntity>().HasOne(m => m.Descendant)
                .WithMany(m => m.MemberRelations)
                .OnDelete(DeleteBehavior.Restrict).IsRequired();
            #endregion

            #region Address
            modelBuilder.Entity<AddressEntity>().HasKey(x => x.Id);
            modelBuilder.Entity<AddressEntity>().Property(x => x.Id).HasMaxLength(128);
            modelBuilder.Entity<AddressEntity>().ToTable("Address");

            modelBuilder.Entity<AddressEntity>().HasOne(m => m.Member).WithMany(m => m.Addresses)
                .HasForeignKey(m => m.MemberId).OnDelete(DeleteBehavior.Cascade).IsRequired();
            #endregion

            #region Email
            modelBuilder.Entity<EmailEntity>().HasKey(x => x.Id);
            modelBuilder.Entity<EmailEntity>().Property(x => x.Id).HasMaxLength(128);
            modelBuilder.Entity<EmailEntity>().ToTable("Email");

            modelBuilder.Entity<EmailEntity>().HasOne(m => m.Member).WithMany(m => m.Emails)
                .HasForeignKey(m => m.MemberId).OnDelete(DeleteBehavior.Cascade).IsRequired();
            modelBuilder.Entity<EmailEntity>().HasIndex(i => i.Address).IsUnique(false).HasName("IX_Email_Address");
            #endregion

            #region Group
            modelBuilder.Entity<MemberGroupEntity>().HasKey(x => x.Id);
            modelBuilder.Entity<MemberGroupEntity>().Property(x => x.Id).HasMaxLength(128);
            modelBuilder.Entity<MemberGroupEntity>().ToTable("MemberGroup");

            modelBuilder.Entity<MemberGroupEntity>().HasOne(m => m.Member).WithMany(m => m.Groups)
                .HasForeignKey(m => m.MemberId).OnDelete(DeleteBehavior.Cascade).IsRequired();
            modelBuilder.Entity<MemberGroupEntity>().HasIndex(i => i.Group).IsUnique(false).HasName("IX_MemberGroup_Group");
            #endregion

            #region Phone
            modelBuilder.Entity<PhoneEntity>().HasKey(x => x.Id);
            modelBuilder.Entity<PhoneEntity>().Property(x => x.Id).HasMaxLength(128);
            modelBuilder.Entity<PhoneEntity>().ToTable("Phone");

            modelBuilder.Entity<PhoneEntity>().HasOne(m => m.Member).WithMany(m => m.Phones)
                .HasForeignKey(m => m.MemberId).OnDelete(DeleteBehavior.Cascade).IsRequired();
            #endregion

            #region Note
            modelBuilder.Entity<NoteEntity>().HasKey(x => x.Id);
            modelBuilder.Entity<NoteEntity>().Property(x => x.Id).HasMaxLength(128);
            modelBuilder.Entity<NoteEntity>().ToTable("Note");

            modelBuilder.Entity<NoteEntity>().HasOne(m => m.Member).WithMany(m => m.Notes)
                .HasForeignKey(m => m.MemberId).OnDelete(DeleteBehavior.Restrict);
            #endregion

            #region Contact
            modelBuilder.Entity<ContactEntity>().Property(p => p.FirstName).HasColumnName("FirstName");
            modelBuilder.Entity<ContactEntity>().Property(p => p.LastName).HasColumnName("LastName");
            modelBuilder.Entity<ContactEntity>().Property(p => p.MiddleName).HasColumnName("MiddleName");
            modelBuilder.Entity<ContactEntity>().Property(p => p.FullName).HasColumnName("FullName");
            modelBuilder.Entity<ContactEntity>().Property(p => p.TimeZone).HasColumnName("TimeZone");
            modelBuilder.Entity<ContactEntity>().Property(p => p.DefaultLanguage).HasColumnName("DefaultLanguage");
            modelBuilder.Entity<ContactEntity>().Property(p => p.PhotoUrl).HasColumnName("PhotoUrl");
            modelBuilder.Entity<ContactEntity>().Property(p => p.BirthDate).HasColumnName("BirthDate");

            #endregion

            #region Organization
            modelBuilder.Entity<OrganizationEntity>().Property(p => p.Description).HasColumnName("Description");
            #endregion

            #region Employee
            modelBuilder.Entity<EmployeeEntity>().Property(p => p.FirstName).HasColumnName("FirstName");
            modelBuilder.Entity<EmployeeEntity>().Property(p => p.LastName).HasColumnName("LastName");
            modelBuilder.Entity<EmployeeEntity>().Property(p => p.MiddleName).HasColumnName("MiddleName");
            modelBuilder.Entity<EmployeeEntity>().Property(p => p.FullName).HasColumnName("FullName");
            modelBuilder.Entity<EmployeeEntity>().Property(p => p.TimeZone).HasColumnName("TimeZone");
            modelBuilder.Entity<EmployeeEntity>().Property(p => p.DefaultLanguage).HasColumnName("DefaultLanguage");
            modelBuilder.Entity<EmployeeEntity>().Property(p => p.PhotoUrl).HasColumnName("PhotoUrl");
            modelBuilder.Entity<EmployeeEntity>().Property(p => p.BirthDate).HasColumnName("BirthDate");

            #endregion

            #region Vendor
            modelBuilder.Entity<VendorEntity>().Property(p => p.Description).HasColumnName("Description");

            #endregion

            #region SeoInfo
            modelBuilder.Entity<SeoInfoEntity>().ToTable("MemberSeoInfo").HasKey(x => x.Id);
            modelBuilder.Entity<SeoInfoEntity>().Property(x => x.Id).HasMaxLength(128);
            modelBuilder.Entity<SeoInfoEntity>().HasOne(x => x.Member).WithMany(x => x.SeoInfos).HasForeignKey(x => x.MemberId)
                .OnDelete(DeleteBehavior.Cascade);

            #endregion

            #region DynamicProperty

            modelBuilder.Entity<MemberDynamicPropertyObjectValueEntity>().ToTable("MemberDynamicPropertyObjectValue").HasKey(x => x.Id);
            modelBuilder.Entity<MemberDynamicPropertyObjectValueEntity>().Property(x => x.Id).HasMaxLength(128);
            modelBuilder.Entity<MemberDynamicPropertyObjectValueEntity>().Property(x => x.DecimalValue).HasColumnType("decimal(18,5)");
            modelBuilder.Entity<MemberDynamicPropertyObjectValueEntity>().HasOne(p => p.Member)
                .WithMany(s => s.DynamicPropertyObjectValues).HasForeignKey(k => k.ObjectId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<MemberDynamicPropertyObjectValueEntity>().HasIndex(x => new { x.ObjectType, x.ObjectId })
                .IsUnique(false)
                .HasName("IX_ObjectType_ObjectId");
            #endregion

            base.OnModelCreating(modelBuilder);
        }
    }
}
