using System.Threading.Tasks;
using VirtoCommerce.PaymentModule.Core.Model;

namespace VirtoCommerce.PaymentModule.Core.Services
{
    public interface IPaymentMethodsService
    {
        Task<PaymentMethod[]> GetByIdsAsync(string[] ids, string responseGroup);
        Task<PaymentMethod> GetByIdAsync(string id, string responseGroup);
        Task SaveChangesAsync(PaymentMethod[] paymentMethods);
        Task DeleteAsync(string[] ids);
    }
}
