using System.Threading.Tasks;
using VirtoCommerce.OrderModule.Core.Model;

namespace VirtoCommerce.OrderModule.Core.Services
{
    public interface ICustomerOrderService 
	{
        Task<CustomerOrder[]> GetByIdsAsync(string[] orderIds, string responseGroup = null);
	    Task<CustomerOrder> GetByIdAsync(string orderId, string responseGroup = null);
        Task SaveChangesAsync(CustomerOrder[] orders);
        Task DeleteAsync(string[] ids);
    }
}
