using System;
using System.Collections.Generic;
using System.Text;
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
            modelBuilder.Entity<StoreEntity>().Property(x => x.CreatedBy).HasMaxLength(64);
            modelBuilder.Entity<StoreEntity>().Property(x => x.ModifiedBy).HasMaxLength(64);
            #endregion

            #region StoreCurrency
            modelBuilder.Entity<StoreCurrencyEntity>().ToTable("StoreCurrency").HasKey(x => x.Id);
            modelBuilder.Entity<StoreCurrencyEntity>().HasOne(x => x.Store).WithMany(x => x.Currencies)
                .HasForeignKey(x => x.StoreId).IsRequired().OnDelete(DeleteBehavior.Cascade);
            #endregion

            #region StoreLanguage
            modelBuilder.Entity<StoreLanguageEntity>().ToTable("StoreLanguage").HasKey(x => x.Id);
            modelBuilder.Entity<StoreLanguageEntity>().HasOne(x => x.Store).WithMany(x => x.Languages)
                .HasForeignKey(x => x.StoreId).IsRequired().OnDelete(DeleteBehavior.Cascade);
            #endregion

            #region StoreTrustedGroups
            modelBuilder.Entity<StoreTrustedGroupEntity>().ToTable("StoreTrustedGroup").HasKey(x => x.Id);
            modelBuilder.Entity<StoreTrustedGroupEntity>().HasOne(x => x.Store).WithMany(x => x.TrustedGroups)
                .HasForeignKey(x => x.StoreId).IsRequired().OnDelete(DeleteBehavior.Cascade);
            #endregion

            #region StorePaymentMethod
            modelBuilder.Entity<StorePaymentMethodEntity>().ToTable("StorePaymentMethod").HasKey(x => x.Id);
            modelBuilder.Entity<StorePaymentMethodEntity>().HasOne(x => x.Store).WithMany(x => x.PaymentMethods)
                .HasForeignKey(x => x.StoreId).IsRequired().OnDelete(DeleteBehavior.Cascade);
            #endregion

            #region StoreShippingMethod
            modelBuilder.Entity<StoreShippingMethodEntity>().ToTable("StoreShippingMethod").HasKey(x => x.Id);
            modelBuilder.Entity<StoreShippingMethodEntity>().HasOne(x => x.Store).WithMany(x => x.ShippingMethods)
                .HasForeignKey(x => x.StoreId).IsRequired().OnDelete(DeleteBehavior.Cascade);
            #endregion

            #region StoreTaxProvider
            modelBuilder.Entity<StoreTaxProviderEntity>().ToTable("StoreTaxProvider").HasKey(x => x.Id);
            modelBuilder.Entity<StoreTaxProviderEntity>().HasOne(x => x.Store).WithMany(x => x.TaxProviders)
                .HasForeignKey(x => x.StoreId).IsRequired().OnDelete(DeleteBehavior.Cascade);
            #endregion

            #region FulfillmentCenters
            modelBuilder.Entity<StoreFulfillmentCenterEntity>().ToTable("StoreFulfillmentCenter").HasKey(x => x.Id);
            modelBuilder.Entity<StoreFulfillmentCenterEntity>().HasOne(x => x.Store).WithMany(x => x.FulfillmentCenters)
                .HasForeignKey(x => x.StoreId).IsRequired().OnDelete(DeleteBehavior.Cascade);
            #endregion
        }
    }
}
