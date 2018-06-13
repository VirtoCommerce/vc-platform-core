using System.Collections.Generic;
using System.Linq;
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

        public IEnumerable<InventoryEntity> GetProductsInventories(IEnumerable<string> productIds)
        {
            return Inventories.Where(x => productIds.Contains(x.Sku)).Include(x => x.FulfillmentCenter).ToArray();
        }

        public IEnumerable<FulfillmentCenterEntity> GetFulfillmentCenters(IEnumerable<string> ids)
        {
            return FulfillmentCenters.Where(x => ids.Contains(x.Id)).ToArray();
        }

        #endregion
    }

}
