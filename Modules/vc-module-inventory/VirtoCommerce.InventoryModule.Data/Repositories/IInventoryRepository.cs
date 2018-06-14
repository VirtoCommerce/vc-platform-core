using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.InventoryModule.Data.Model;
using VirtoCommerce.Platform.Core.Common;
namespace VirtoCommerce.InventoryModule.Data.Repositories
{
    public interface IInventoryRepository : IRepository
	{
		IQueryable<InventoryEntity> Inventories { get; }
        IQueryable<FulfillmentCenterEntity> FulfillmentCenters { get; }

	    Task<IEnumerable<InventoryEntity>> GetProductsInventories(IEnumerable<string> productIds);
        Task<IEnumerable<FulfillmentCenterEntity>> GetFulfillmentCenters(IEnumerable<string> ids);
	    Task<IEnumerable<InventoryEntity>> GetByIdsAsync(string[] ids);
	}
}
