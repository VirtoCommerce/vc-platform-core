using EntityFrameworkCore.Triggers;
using Microsoft.EntityFrameworkCore;
using VirtoCommerce.PricingModule.Data.Model;

namespace VirtoCommerce.PricingModule.Data.Repositories
{
    public class PricingDbContext : DbContextWithTriggers
    {
        public PricingDbContext(DbContextOptions<PricingDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PriceEntity>().ToTable("Price").HasKey(x => x.Id);
            modelBuilder.Entity<PriceEntity>().Property(x => x.Id);
            modelBuilder.Entity<PriceEntity>().HasOne(x => x.Pricelist).WithMany(x => x.Prices).IsRequired().HasForeignKey(x => x.PricelistId);
            modelBuilder.Entity<PriceEntity>().HasIndex(x => new { x.ProductId, x.PricelistId }).HasName("ProductIdAndPricelistId");
            modelBuilder.Entity<PriceEntity>();

            modelBuilder.Entity<PricelistEntity>().ToTable("Pricelist").HasKey(x => x.Id);
            modelBuilder.Entity<PricelistEntity>().Property(x => x.Id);

            modelBuilder.Entity<PricelistAssignmentEntity>().ToTable("PricelistAssignment").HasKey(x => x.Id);
            modelBuilder.Entity<PricelistAssignmentEntity>().HasOne(x => x.Pricelist).WithMany(x => x.Assignments).IsRequired().HasForeignKey(x => x.PricelistId);
            modelBuilder.Entity<PricelistAssignmentEntity>().Property(x => x.Id);

            base.OnModelCreating(modelBuilder);
        }
    }
}
