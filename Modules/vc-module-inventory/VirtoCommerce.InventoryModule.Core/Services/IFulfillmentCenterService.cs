using System.Collections.Generic;
using System.Threading.Tasks;
using VirtoCommerce.InventoryModule.Core.Model;

namespace VirtoCommerce.InventoryModule.Core.Services
{
    public interface IFulfillmentCenterService
    {
        Task SaveChangesAsync(IEnumerable<FulfillmentCenter> fulfillmentCenters);
        Task<IEnumerable<FulfillmentCenter>> GetByIdsAsync(IEnumerable<string> ids);
        Task DeleteAsync(IEnumerable<string> ids);
    }
}
