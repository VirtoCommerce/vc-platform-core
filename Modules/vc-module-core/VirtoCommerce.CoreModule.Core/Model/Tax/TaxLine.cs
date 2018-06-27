using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CoreModule.Core.Model.Tax
{
    /// <summary>
    /// Represent one abstract position for tax calculation
    /// </summary>
    public class TaxLine : Entity
    {
        public string Code { get; set; }
        public string Name { get; set; }
        /// <summary>
        /// Total sum  
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// Quantity
        /// </summary>
        public int Quantity { get; set; }
        /// <summary>
        /// Price per item
        /// </summary>
        public decimal Price { get; set; }
        /// <summary>
        /// Tax category/type
        /// </summary>
        public string TaxType { get; set; }
    }
}
