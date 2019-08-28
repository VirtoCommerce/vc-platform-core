using EntityFrameworkCore.Triggers;
using Microsoft.EntityFrameworkCore;
using VirtoCommerce.TaxModule.Data.Model;

namespace VirtoCommerce.TaxModule.Data.Repositories
{
    public class TaxDbContext : DbContextWithTriggers
    {
        public TaxDbContext(DbContextOptions<TaxDbContext> options) : base(options)
        {
        }

        protected TaxDbContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region StoreTaxProvider
            modelBuilder.Entity<StoreTaxProviderEntity>().ToTable("StoreTaxProvider").HasKey(x => x.Id);
            modelBuilder.Entity<StoreTaxProviderEntity>().Property(x => x.Id).HasMaxLength(128);
            modelBuilder.Entity<StoreTaxProviderEntity>().Property(x => x.StoreId).HasMaxLength(128);

            modelBuilder.Entity<StoreTaxProviderEntity>().HasIndex(x => new { x.TypeName, x.StoreId })
                      .HasName("IX_StoreTaxProviderEntity_TypeName_StoreId")
                      .IsUnique(true);
            #endregion
        }
    }
}
