using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Data.Infrastructure;
using VirtoCommerce.StoreModule.Core.Model;
using VirtoCommerce.StoreModule.Data.Model;

namespace VirtoCommerce.StoreModule.Data.Repositories
{
    public class StoreRepository : DbContextRepositoryBase<StoreDbContext>, IStoreRepository
    {
        public StoreRepository(StoreDbContext dbContext) : base(dbContext)
        {
        }

        #region IStoreRepository Members

        public async Task<StoreEntity[]> GetStoresByIdsAsync(string[] ids, string responseGroup = null)
        {
            var storeResponseGroup = EnumUtility.SafeParseFlags(responseGroup, StoreResponseGroup.Full);

            var retVal = await Stores.Where(x => ids.Contains(x.Id))
                               .Include(x => x.Languages)
                               .Include(x => x.Currencies)
                               .Include(x => x.TrustedGroups)
                               .ToArrayAsync();

            if (storeResponseGroup.HasFlag(StoreResponseGroup.StoreFulfillmentCenters))
            {
                var fulfillmentCenters = await StoreFulfillmentCenters.Where(x => ids.Contains(x.StoreId)).ToArrayAsync();
            }

            if (storeResponseGroup.HasFlag(StoreResponseGroup.StoreSeoInfos))
            {
                var seoInfos = await SeoInfos.Where(x => ids.Contains(x.StoreId)).ToArrayAsync();
            }

            if (storeResponseGroup.HasFlag(StoreResponseGroup.DynamicProperties))
            {
                var dynamicPropertyValues = await DynamicPropertyObjectValues.Where(x => ids.Contains(x.ObjectId)).ToArrayAsync();
            }

            return retVal;
        }

        public IQueryable<StoreEntity> Stores => DbContext.Set<StoreEntity>();
        public IQueryable<StoreFulfillmentCenterEntity> StoreFulfillmentCenters => DbContext.Set<StoreFulfillmentCenterEntity>();

        public IQueryable<SeoInfoEntity> SeoInfos => DbContext.Set<SeoInfoEntity>();
        public IQueryable<StoreDynamicPropertyObjectValueEntity> DynamicPropertyObjectValues => DbContext.Set<StoreDynamicPropertyObjectValueEntity>();

        #endregion
    }
}
