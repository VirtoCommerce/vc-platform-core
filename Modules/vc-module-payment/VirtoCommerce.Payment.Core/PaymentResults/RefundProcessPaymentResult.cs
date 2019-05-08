using VirtoCommerce.PaymentModule.Core.Models;

namespace VirtoCommerce.PaymentModule.Core.PaymentResults
{
    public class RefundProcessPaymentResult : IProcessResult
    {
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }
        public PaymentStatus NewPaymentStatus { get; set; }
    }
}
