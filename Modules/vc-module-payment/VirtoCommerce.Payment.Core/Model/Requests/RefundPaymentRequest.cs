namespace VirtoCommerce.PaymentModule.Model.Requests
{
    public class RefundPaymentRequest : PaymentRequestBase
    {
        /// <summary>
        /// Gets or sets an amount
        /// </summary>
        public decimal AmountToRefund { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether it's a partial refund; otherwise, full refund
        /// </summary>
        public bool IsPartialRefund { get; set; }
    }
}
