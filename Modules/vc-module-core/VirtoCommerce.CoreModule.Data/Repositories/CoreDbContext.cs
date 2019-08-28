using EntityFrameworkCore.Triggers;
using Microsoft.EntityFrameworkCore;
using VirtoCommerce.CoreModule.Data.Currency;
using VirtoCommerce.CoreModule.Data.Model;
using VirtoCommerce.CoreModule.Data.Package;

namespace VirtoCommerce.CoreModule.Data.Repositories
{
    public class CoreDbContext : DbContextWithTriggers
    {
        public CoreDbContext(DbContextOptions<CoreDbContext> options)
            : base(options)
        {
        }

        protected CoreDbContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SequenceEntity>().ToTable("Sequence").HasKey(x => x.ObjectType);

            modelBuilder.Entity<CurrencyEntity>().ToTable("Currency").HasKey(x => x.Id);
            modelBuilder.Entity<CurrencyEntity>().Property(x => x.Id).HasMaxLength(128);
            modelBuilder.Entity<CurrencyEntity>().HasIndex(x => x.Code).HasName("IX_Code");

            modelBuilder.Entity<PackageTypeEntity>().ToTable("PackageType").HasKey(x => x.Id);
            modelBuilder.Entity<PackageTypeEntity>().Property(x => x.Id).HasMaxLength(128);

            base.OnModelCreating(modelBuilder);
        }
    }
}
