using VirtoCommerce.PaymentModule.Core.Model;

namespace VirtoCommerce.PaymentModule.Model.Requests
{
    public interface IPaymenRequestResult
    {
        bool IsSuccess { get; set; }
        string ErrorMessage { get; set; }
        PaymentStatus NewPaymentStatus { get; set; }
    }
}
