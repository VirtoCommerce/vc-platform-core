namespace VirtoCommerce.CoreModule.Core.Payment
{
    public class CaptureProcessPaymentResult : IProcessResult
    {
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }
        public PaymentStatus NewPaymentStatus { get; set; }
        public string OuterId { get; set; }
    }
}
