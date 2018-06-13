using System.Collections.Generic;
using System.Threading.Tasks;
using VirtoCommerce.InventoryModule.Core.Model;

namespace VirtoCommerce.InventoryModule.Core.Services
{
	public interface IInventoryService
	{
        IEnumerable<InventoryInfo> GetProductsInventoryInfos(IEnumerable<string> productIds);
		Task<InventoryInfo> UpsertInventoryAsync(InventoryInfo inventoryInfo);
		Task UpsertInventoriesAsync(IEnumerable<InventoryInfo> inventoryInfos);
	}
}
