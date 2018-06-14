using System.Collections.Generic;
using System.Threading.Tasks;
using VirtoCommerce.InventoryModule.Core.Model;

namespace VirtoCommerce.InventoryModule.Core.Services
{
	public interface IInventoryService
	{
        Task<IEnumerable<InventoryInfo>> GetProductsInventoryInfosAsync(IEnumerable<string> productIds);
		Task<InventoryInfo> UpsertInventoryAsync(InventoryInfo inventoryInfo);
		Task UpsertInventoriesAsync(IEnumerable<InventoryInfo> inventoryInfos);
	    Task<IEnumerable<InventoryInfo>> GetByIdsAsync(string[] ids);
    }
}
