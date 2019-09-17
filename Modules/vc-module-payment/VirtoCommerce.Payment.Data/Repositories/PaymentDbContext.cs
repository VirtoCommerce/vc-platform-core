using Microsoft.EntityFrameworkCore;
using VirtoCommerce.PaymentModule.Data.Model;
using VirtoCommerce.Platform.Data.Infrastructure;

namespace VirtoCommerce.PaymentModule.Data.Repositories
{
    public class PaymentDbContext : DbContextWithTriggersAndQueryFiltersBase
    {
        public PaymentDbContext(DbContextOptions<PaymentDbContext> options) : base(options)
        {
        }

        protected PaymentDbContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<StorePaymentMethodEntity>().ToTable("StorePaymentMethod").HasKey(x => x.Id);
            modelBuilder.Entity<StorePaymentMethodEntity>().Property(x => x.Id).HasMaxLength(128);
            modelBuilder.Entity<StorePaymentMethodEntity>().Property(x => x.StoreId).HasMaxLength(128);
            modelBuilder.Entity<StorePaymentMethodEntity>().Property(x => x.TypeName).HasMaxLength(128);

            modelBuilder.Entity<StorePaymentMethodEntity>().HasIndex(x => new { x.TypeName, x.StoreId })
                .HasName("IX_StorePaymentMethodEntity_TypeName_StoreId")
                .IsUnique();
        }
    }
}
