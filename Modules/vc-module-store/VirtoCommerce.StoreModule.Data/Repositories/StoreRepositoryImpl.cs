using System.Data.Entity;
using System.Linq;
using VirtoCommerce.Platform.Data.Infrastructure;
using VirtoCommerce.Platform.Data.Infrastructure.Interceptors;
using VirtoCommerce.StoreModule.Data.Model;

namespace VirtoCommerce.StoreModule.Data.Repositories
{
    public class StoreRepositoryImpl : EFRepositoryBase, IStoreRepository
    {
        public StoreRepositoryImpl()
        {
        }

        public StoreRepositoryImpl(string nameOrConnectionString, params IInterceptor[] interceptors)
            : base(nameOrConnectionString, null, interceptors)
        {
            Configuration.LazyLoadingEnabled = false;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            #region Store
            modelBuilder.Entity<StoreEntity>().HasKey(x => x.Id)
                .Property(x => x.Id);
            modelBuilder.Entity<StoreEntity>().ToTable("Store");
            #endregion

            #region StoreCurrency
            modelBuilder.Entity<StoreCurrencyEntity>().HasKey(x => x.Id)
                .Property(x => x.Id);
            modelBuilder.Entity<StoreCurrencyEntity>().ToTable("StoreCurrency");

            modelBuilder.Entity<StoreCurrencyEntity>().HasRequired(x => x.Store)
                                   .WithMany(x => x.Currencies)
                                   .HasForeignKey(x => x.StoreId).WillCascadeOnDelete(true);
            #endregion

            #region StoreLanguage
            modelBuilder.Entity<StoreLanguageEntity>().HasKey(x => x.Id)
                .Property(x => x.Id);
            modelBuilder.Entity<StoreLanguageEntity>().ToTable("StoreLanguage");

            modelBuilder.Entity<StoreLanguageEntity>().HasRequired(x => x.Store)
                                   .WithMany(x => x.Languages)
                                   .HasForeignKey(x => x.StoreId).WillCascadeOnDelete(true);
            #endregion

            #region StoreTrustedGroups
            modelBuilder.Entity<StoreTrustedGroupEntity>().HasKey(x => x.Id)
                .Property(x => x.Id);
            modelBuilder.Entity<StoreTrustedGroupEntity>().ToTable("StoreTrustedGroup");

            modelBuilder.Entity<StoreTrustedGroupEntity>().HasRequired(x => x.Store)
                                   .WithMany(x => x.TrustedGroups)
                                   .HasForeignKey(x => x.StoreId).WillCascadeOnDelete(true);
            #endregion

            #region StorePaymentMethod
            modelBuilder.Entity<StorePaymentMethodEntity>().HasKey(x => x.Id)
                .Property(x => x.Id);
            modelBuilder.Entity<StorePaymentMethodEntity>().ToTable("StorePaymentMethod");

            modelBuilder.Entity<StorePaymentMethodEntity>().HasRequired(x => x.Store)
                               .WithMany(x => x.PaymentMethods)
                               .HasForeignKey(x => x.StoreId).WillCascadeOnDelete(true);
            #endregion

            #region StoreShippingMethod
            modelBuilder.Entity<StoreShippingMethodEntity>().HasKey(x => x.Id)
                .Property(x => x.Id);
            modelBuilder.Entity<StoreShippingMethodEntity>().ToTable("StoreShippingMethod");

            modelBuilder.Entity<StoreShippingMethodEntity>().HasRequired(x => x.Store)
                                   .WithMany(x => x.ShippingMethods)
                                   .HasForeignKey(x => x.StoreId).WillCascadeOnDelete(true);
            #endregion

            #region StoreTaxProvider
            modelBuilder.Entity<StoreTaxProviderEntity>().HasKey(x => x.Id)
                .Property(x => x.Id);
            modelBuilder.Entity<StoreTaxProviderEntity>().ToTable("StoreTaxProvider");

            modelBuilder.Entity<StoreTaxProviderEntity>().HasRequired(x => x.Store)
                                 .WithMany(x => x.TaxProviders)
                                 .HasForeignKey(x => x.StoreId).WillCascadeOnDelete(true);
            #endregion

            #region FulfillmentCenters
            modelBuilder.Entity<StoreFulfillmentCenterEntity>().HasKey(x => x.Id).Property(x => x.Id);
            modelBuilder.Entity<StoreFulfillmentCenterEntity>().ToTable("StoreFulfillmentCenter");
            modelBuilder.Entity<StoreFulfillmentCenterEntity>()
                .HasRequired(x => x.Store)
                .WithMany(x => x.FulfillmentCenters)
                .HasForeignKey(x => x.StoreId)
                .WillCascadeOnDelete(true);
            #endregion

            base.OnModelCreating(modelBuilder);
        }

        #region IStoreRepository Members

        public StoreEntity[] GetStoresByIds(string[] ids)
        {
            var retVal = Stores.Where(x => ids.Contains(x.Id))
                               .Include(x => x.Languages)
                               .Include(x => x.Currencies)
                               .Include(x => x.TrustedGroups)
                               .ToArray();
            var paymentMethods = StorePaymentMethods.Where(x => ids.Contains(x.StoreId)).ToArray();
            var shipmentMethods = StoreShippingMethods.Where(x => ids.Contains(x.StoreId)).ToArray();
            var taxProviders = StoreTaxProviders.Where(x => ids.Contains(x.StoreId)).ToArray();
            var fulfillmentCenters = StoreFulfillmentCenters.Where(x => ids.Contains(x.StoreId)).ToArray();

            return retVal;
        }

        public IQueryable<StoreEntity> Stores
        {
            get { return GetAsQueryable<StoreEntity>(); }
        }
        public IQueryable<StorePaymentMethodEntity> StorePaymentMethods
        {
            get { return GetAsQueryable<StorePaymentMethodEntity>(); }
        }
        public IQueryable<StoreShippingMethodEntity> StoreShippingMethods
        {
            get { return GetAsQueryable<StoreShippingMethodEntity>(); }
        }
        public IQueryable<StoreTaxProviderEntity> StoreTaxProviders
        {
            get { return GetAsQueryable<StoreTaxProviderEntity>(); }
        }
        public IQueryable<StoreFulfillmentCenterEntity> StoreFulfillmentCenters
        {
            get { return GetAsQueryable<StoreFulfillmentCenterEntity>(); }
        }
        #endregion
    }
}