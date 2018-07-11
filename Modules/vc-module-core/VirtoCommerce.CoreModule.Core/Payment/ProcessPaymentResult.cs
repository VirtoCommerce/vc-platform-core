using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CoreModule.Core.Payment
{
    public class ProcessPaymentResult : ValueObject
    {
        public PaymentMethod PaymentMethod { get; set; }

        public PaymentStatus NewPaymentStatus { get; set; }

        public string RedirectUrl { get; set; }

        public string HtmlForm { get; set; }

        public bool IsSuccess { get; set; }

        public string Error { get; set; }

        public string OuterId { get; set; }
    }
}
