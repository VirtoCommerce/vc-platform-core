namespace VirtoCommerce.PaymentModule.Model.Requests
{
    public class ProcessPaymentRequestResult : PaymentRequestResultBase
    {
        public string RedirectUrl { get; set; }

        public string HtmlForm { get; set; }

        public string OuterId { get; set; }
    }
}
