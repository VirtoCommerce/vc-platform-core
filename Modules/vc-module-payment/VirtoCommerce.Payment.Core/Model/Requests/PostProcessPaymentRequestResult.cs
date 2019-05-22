namespace VirtoCommerce.PaymentModule.Model.Requests
{
    public class PostProcessPaymentRequestResult : PaymentRequestResultBase
    {
        public string ReturnUrl { get; set; }

        public string OrderId { get; set; }

        public string OuterId { get; set; }
    }
}
