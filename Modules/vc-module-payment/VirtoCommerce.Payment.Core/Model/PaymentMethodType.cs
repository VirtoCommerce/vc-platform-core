namespace VirtoCommerce.PaymentModule.Core.Model
{
    public enum PaymentMethodType
    {
        Unknown,
        /// <summary>
        /// All payment information is entered on the site
        /// </summary>
        Standard,
        /// <summary>
        /// A customer is redirected to a third-party site in order to complete the payment
        /// </summary>
        Redirection,
        /// <summary>
        /// Payment system send prepared html form for request
        /// </summary>
        PreparedForm
    }
}
