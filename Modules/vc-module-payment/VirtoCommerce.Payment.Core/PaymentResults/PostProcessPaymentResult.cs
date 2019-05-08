using VirtoCommerce.PaymentModule.Core.Models;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.PaymentModule.Core.PaymentResults
{
    public class PostProcessPaymentResult : ValueObject, IProcessResult
    {
        public PaymentStatus NewPaymentStatus { get; set; }

        public bool IsSuccess { get; set; }

        public string ErrorMessage { get; set; }

        public string ReturnUrl { get; set; }

        public string OrderId { get; set; }

        public string OuterId { get; set; }
    }
}
