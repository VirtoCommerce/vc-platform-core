using VirtoCommerce.PaymentModule.Core.Models;

namespace VirtoCommerce.PaymentModule.Core.PaymentResults
{
    public interface IProcessResult
    {
        bool IsSuccess { get; set; }
        string ErrorMessage { get; set; }
        PaymentStatus NewPaymentStatus { get; set; }
    }
}
