using System.Threading.Tasks;
using VirtoCommerce.PaymentModule.Core.Models.Search;

namespace VirtoCommerce.PaymentModule.Core.Services
{
    public interface IPaymentMethodsSearchService
    {
        Task<PaymentMethodsSearchResult> SearchPaymentMethodsAsync(PaymentMethodsSearchCriteria criteria);
    }
}
