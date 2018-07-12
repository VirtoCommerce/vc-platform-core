namespace VirtoCommerce.CoreModule.Core.Payment
{
    public interface IProcessResult
    {
        bool IsSuccess { get; set; }
        string ErrorMessage { get; set; }
        PaymentStatus NewPaymentStatus { get; set; }
    }
}
