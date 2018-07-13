namespace VirtoCommerce.CoreModule.Core.Payment
{
    public class VoidProcessPaymentResult : IProcessResult
    {
        public bool IsSuccess { get; set; }

        public string ErrorMessage { get; set; }

        public PaymentStatus NewPaymentStatus { get; set; }
    }
}
