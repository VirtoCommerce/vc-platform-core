using EntityFrameworkCore.Triggers;
using Microsoft.EntityFrameworkCore;
using VirtoCommerce.StoreModule.Data.Model;

namespace VirtoCommerce.StoreModule.Data.Repositories
{
    public class StoreDbContext : DbContextWithTriggers
    {
        public StoreDbContext(DbContextOptions<StoreDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region Store
            modelBuilder.Entity<StoreEntity>().ToTable("Store").HasKey(x => x.Id);
            modelBuilder.Entity<StoreEntity>().Property(x => x.Id).HasMaxLength(128);
            #endregion

            #region StoreCurrency
            modelBuilder.Entity<StoreCurrencyEntity>().ToTable("StoreCurrency").HasKey(x => x.Id);
            modelBuilder.Entity<StoreCurrencyEntity>().Property(x => x.Id).HasMaxLength(128);
            modelBuilder.Entity<StoreCurrencyEntity>().HasOne(x => x.Store).WithMany(x => x.Currencies)
                .HasForeignKey(x => x.StoreId).IsRequired().OnDelete(DeleteBehavior.Cascade);
            #endregion

            #region StoreLanguage
            modelBuilder.Entity<StoreLanguageEntity>().ToTable("StoreLanguage").HasKey(x => x.Id);
            modelBuilder.Entity<StoreLanguageEntity>().Property(x => x.Id).HasMaxLength(128);
            modelBuilder.Entity<StoreLanguageEntity>().HasOne(x => x.Store).WithMany(x => x.Languages)
                .HasForeignKey(x => x.StoreId).IsRequired().OnDelete(DeleteBehavior.Cascade);
            #endregion

            #region StoreTrustedGroups
            modelBuilder.Entity<StoreTrustedGroupEntity>().ToTable("StoreTrustedGroup").HasKey(x => x.Id);
            modelBuilder.Entity<StoreTrustedGroupEntity>().Property(x => x.Id).HasMaxLength(128);
            modelBuilder.Entity<StoreTrustedGroupEntity>().HasOne(x => x.Store).WithMany(x => x.TrustedGroups)
                .HasForeignKey(x => x.StoreId).IsRequired().OnDelete(DeleteBehavior.Cascade);
            #endregion

            #region StorePaymentMethod
            modelBuilder.Entity<StorePaymentMethodEntity>().ToTable("StorePaymentMethod").HasKey(x => x.Id);
            modelBuilder.Entity<StorePaymentMethodEntity>().Property(x => x.Id).HasMaxLength(128);
            modelBuilder.Entity<StorePaymentMethodEntity>().HasOne(x => x.Store).WithMany(x => x.PaymentMethods)
                .HasForeignKey(x => x.StoreId).IsRequired().OnDelete(DeleteBehavior.Cascade);
            #endregion

            #region StoreShippingMethod
            modelBuilder.Entity<StoreShippingMethodEntity>().ToTable("StoreShippingMethod").HasKey(x => x.Id);
            modelBuilder.Entity<StoreShippingMethodEntity>().Property(x => x.Id).HasMaxLength(128);
            modelBuilder.Entity<StoreShippingMethodEntity>().HasOne(x => x.Store).WithMany(x => x.ShippingMethods)
                .HasForeignKey(x => x.StoreId).IsRequired().OnDelete(DeleteBehavior.Cascade);
            #endregion


            #region FulfillmentCenters
            modelBuilder.Entity<StoreFulfillmentCenterEntity>().ToTable("StoreFulfillmentCenter").HasKey(x => x.Id);
            modelBuilder.Entity<StoreFulfillmentCenterEntity>().Property(x => x.Id).HasMaxLength(128);
            modelBuilder.Entity<StoreFulfillmentCenterEntity>().HasOne(x => x.Store).WithMany(x => x.FulfillmentCenters)
                .HasForeignKey(x => x.StoreId).IsRequired().OnDelete(DeleteBehavior.Cascade);
            #endregion

            #region SeoInfo
            modelBuilder.Entity<SeoInfoEntity>().ToTable("StoreSeoInfo").HasKey(x => x.Id);
            modelBuilder.Entity<SeoInfoEntity>().Property(x => x.Id).HasMaxLength(128);
            modelBuilder.Entity<SeoInfoEntity>().HasOne(x => x.Store).WithMany(x => x.SeoInfos).HasForeignKey(x => x.StoreId)
                .OnDelete(DeleteBehavior.Cascade);

            #endregion
        }
    }
}
