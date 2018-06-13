using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.InventoryModule.Data.Model;
using VirtoCommerce.Platform.Core.Common;
namespace VirtoCommerce.InventoryModule.Data.Repositories
{
    public interface IInventoryRepository : IRepository
	{
		IQueryable<InventoryEntity> Inventories { get; }
        IQueryable<FulfillmentCenterEntity> FulfillmentCenters { get; }

        IEnumerable<InventoryEntity> GetProductsInventories(IEnumerable<string> productIds);
        IEnumerable<FulfillmentCenterEntity> GetFulfillmentCenters(IEnumerable<string> ids);
    }
}
