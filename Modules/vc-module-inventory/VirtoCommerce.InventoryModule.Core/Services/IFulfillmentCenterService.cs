using System.Collections.Generic;
using System.Threading.Tasks;
using VirtoCommerce.InventoryModule.Core.Model;

namespace VirtoCommerce.InventoryModule.Core.Services
{
    public interface IFulfillmentCenterService
    {
        Task SaveChangesAsync(IEnumerable<FulfillmentCenter> fulfillmentCenters);
        IEnumerable<FulfillmentCenter> GetByIds(IEnumerable<string> ids, string responseGroup = null);
        Task DeleteAsync(IEnumerable<string> ids);
    }
}
