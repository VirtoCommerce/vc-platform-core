using VirtoCommerce.PaymentModule.Core.Model;

namespace VirtoCommerce.PaymentModule.Model.Requests
{
    public class ProcessPaymentRequest : PaymentRequestBase
    {
        public BankCardInfo BankCardInfo { get; set; }
    }
}
