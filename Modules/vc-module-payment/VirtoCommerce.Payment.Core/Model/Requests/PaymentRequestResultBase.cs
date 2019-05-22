using VirtoCommerce.PaymentModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.PaymentModule.Model.Requests
{
    public class PaymentRequestResultBase : ValueObject, IPaymenRequestResult
    {
        public PaymentMethod PaymentMethod { get; set; }
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }
        public PaymentStatus NewPaymentStatus { get; set; }
    }
}
