using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VirtoCommerce.Platform.Data.Infrastructure;
using VirtoCommerce.StoreModule.Data.Model;

namespace VirtoCommerce.StoreModule.Data.Repositories
{
    public class StoreRepository : DbContextRepositoryBase<StoreDbContext>, IStoreRepository
    {
        public StoreRepository(StoreDbContext dbContext) : base(dbContext)
        {
        }

        #region IStoreRepository Members

        public async Task<StoreEntity[]> GetStoresByIdsAsync(string[] ids)
        {
            var retVal = await Stores.Where(x => ids.Contains(x.Id))
                               .Include(x => x.Languages)
                               .Include(x => x.Currencies)
                               .Include(x => x.TrustedGroups)
                               .ToArrayAsync();
            var paymentMethods = StorePaymentMethods.Where(x => ids.Contains(x.StoreId)).ToArrayAsync();
            var shipmentMethods = StoreShippingMethods.Where(x => ids.Contains(x.StoreId)).ToArrayAsync();
            var fulfillmentCenters = StoreFulfillmentCenters.Where(x => ids.Contains(x.StoreId)).ToArrayAsync();
            var seoInfos = SeoInfos.Where(x => ids.Contains(x.StoreId)).ToArrayAsync();

            await Task.WhenAll(paymentMethods, shipmentMethods, fulfillmentCenters, seoInfos);

            return retVal;
        }

        public IQueryable<StoreEntity> Stores => DbContext.Set<StoreEntity>();
        public IQueryable<StorePaymentMethodEntity> StorePaymentMethods => DbContext.Set<StorePaymentMethodEntity>();
        public IQueryable<StoreShippingMethodEntity> StoreShippingMethods => DbContext.Set<StoreShippingMethodEntity>();
        public IQueryable<StoreFulfillmentCenterEntity> StoreFulfillmentCenters => DbContext.Set<StoreFulfillmentCenterEntity>();
        public IQueryable<SeoInfoEntity> SeoInfos => DbContext.Set<SeoInfoEntity>();


        #endregion


    }
}
