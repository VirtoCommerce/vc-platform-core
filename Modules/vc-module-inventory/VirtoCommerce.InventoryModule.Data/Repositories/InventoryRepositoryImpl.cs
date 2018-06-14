using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.InventoryModule.Data.Model;
using Microsoft.EntityFrameworkCore;
using VirtoCommerce.Platform.Core.Domain;
using VirtoCommerce.Platform.Data.Infrastructure;

namespace VirtoCommerce.InventoryModule.Data.Repositories
{
    public class InventoryRepositoryImpl : DbContextRepositoryBase<InventoryDbContext>, IInventoryRepository
    {
        public InventoryRepositoryImpl(InventoryDbContext dbContext, IUnitOfWork unitOfWork = null) : base(dbContext, unitOfWork)
        {
        }

        #region IFoundationInventoryRepository Members

        public IQueryable<InventoryEntity> Inventories => DbContext.Set<InventoryEntity>();

        public IQueryable<FulfillmentCenterEntity> FulfillmentCenters => DbContext.Set<FulfillmentCenterEntity>();

        public async Task<IEnumerable<InventoryEntity>> GetProductsInventories(IEnumerable<string> productIds)
        {
            return await Inventories.Where(x => productIds.Contains(x.Sku)).Include(x => x.FulfillmentCenter).ToListAsync();
        }

        public async Task<IEnumerable<FulfillmentCenterEntity>> GetFulfillmentCenters(IEnumerable<string> ids)
        {
            return await FulfillmentCenters.Where(x => ids.Contains(x.Id)).ToListAsync();
        }

        public async Task<IEnumerable<InventoryEntity>> GetByIdsAsync(string[] ids)
        {
            return await Inventories.Where(x => ids.Contains(x.Id)).ToListAsync();
        }

        #endregion
    }

}
