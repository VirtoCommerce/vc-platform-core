using EntityFrameworkCore.Triggers;
using Microsoft.EntityFrameworkCore;
using VirtoCommerce.PaymentModule.Data.Model;

namespace VirtoCommerce.PaymentModule.Data.Repositories
{
    public class PaymentDbContext : DbContextWithTriggers
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
