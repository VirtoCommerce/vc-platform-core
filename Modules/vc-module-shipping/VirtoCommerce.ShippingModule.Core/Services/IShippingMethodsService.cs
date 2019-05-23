using System.Threading.Tasks;
using VirtoCommerce.ShippingModule.Core.Model;

namespace VirtoCommerce.ShippingModule.Core.Services
{
    public interface IShippingMethodsService
    {
        Task<ShippingMethod[]> GetByIdsAsync(string[] ids, string responseGroup);
        Task<ShippingMethod> GetByIdAsync(string id, string responseGroup);
        Task SaveChangesAsync(ShippingMethod[] shippingMethods);
        Task DeleteAsync(string[] ids);
    }
}
