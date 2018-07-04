using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CoreModule.Core.Commerce.Model
{
    /// <summary>
    /// Currency
    /// </summary>
    public class Currency : ValueObject
    {
        /// <summary>
        /// the currency code
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        ///  name of the currency
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Flag specifies that this is the primary currency
        /// </summary>
        public bool IsPrimary { get; set; }
        /// <summary>
        /// The exchange rate against the primary exchange rate of the currency.
        /// </summary>
        public decimal ExchangeRate { get; set; }
        /// <summary>
        /// Currency symbol
        /// </summary>
        public string Symbol { get; set; }

        /// <summary>
        /// Custom formatting pattern
        /// </summary>
        public string CustomFormatting { get; set; }
    }
}
