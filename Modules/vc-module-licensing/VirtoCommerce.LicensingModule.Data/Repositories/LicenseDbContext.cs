using EntityFrameworkCore.Triggers;
using Microsoft.EntityFrameworkCore;
using VirtoCommerce.LicensingModule.Data.Model;

namespace VirtoCommerce.LicensingModule.Data.Repositories
{
    public class LicenseDbContext : DbContextWithTriggers
    {
        public LicenseDbContext(DbContextOptions<LicenseDbContext> options)
            : base(options)
        {
        }

        protected LicenseDbContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LicenseEntity>().HasKey(x => x.Id);
            modelBuilder.Entity<LicenseEntity>().Property(x => x.Id).HasMaxLength(128);
            modelBuilder.Entity<LicenseEntity>().ToTable("License");
            modelBuilder.Entity<LicenseEntity>().HasIndex(x => x.ActivationCode).HasName("IX_ActivationCode").IsUnique();

            base.OnModelCreating(modelBuilder);
        }
    }
}
