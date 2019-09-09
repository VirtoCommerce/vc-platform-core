using Microsoft.EntityFrameworkCore;
using VirtoCommerce.InventoryModule.Data.Model;
using VirtoCommerce.Platform.Data.Infrastructure;

namespace VirtoCommerce.InventoryModule.Data.Repositories
{
    public class InventoryDbContext : DbContextWithTriggersAndQueryFiltersBase
    {
        public InventoryDbContext(DbContextOptions<InventoryDbContext> options)
            : base(options)
        {
        }

        protected InventoryDbContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<InventoryEntity>().ToTable("Inventory").HasKey(x => x.Id);
            modelBuilder.Entity<InventoryEntity>().Property(x => x.Id).HasMaxLength(128);
            modelBuilder.Entity<InventoryEntity>().HasIndex(inv => new { inv.Sku }).IsUnique(false);
            modelBuilder.Entity<InventoryEntity>().HasOne(x => x.FulfillmentCenter).WithMany()
                .HasForeignKey(x => x.FulfillmentCenterId).IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<FulfillmentCenterEntity>().ToTable("FulfillmentCenter").HasKey(x => x.Id);
            modelBuilder.Entity<FulfillmentCenterEntity>().Property(x => x.Id).HasMaxLength(128);


            base.OnModelCreating(modelBuilder);
        }
    }
}
