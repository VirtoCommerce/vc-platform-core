using EntityFrameworkCore.Triggers;
using Microsoft.EntityFrameworkCore;
using VirtoCommerce.ShippingModule.Data.Model;

namespace VirtoCommerce.ShippingModule.Data.Repositories
{
    public class ShippingDbContext : DbContextWithTriggers
    {
        public ShippingDbContext(DbContextOptions<ShippingDbContext> options) : base(options)
        {
        }

        protected ShippingDbContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<StoreShippingMethodEntity>().ToTable("StoreShippingMethod").HasKey(x => x.Id);
            modelBuilder.Entity<StoreShippingMethodEntity>().Property(x => x.Id).HasMaxLength(128);
            modelBuilder.Entity<StoreShippingMethodEntity>().Property(x => x.StoreId).HasMaxLength(128);
            modelBuilder.Entity<StoreShippingMethodEntity>().Property(x => x.TypeName).HasMaxLength(128);
            modelBuilder.Entity<StoreShippingMethodEntity>().HasIndex(x => new { x.TypeName, x.StoreId })
                .HasName("IX_StoreShippingMethodEntity_TypeName_StoreId")
                .IsUnique();
        }
    }
}
