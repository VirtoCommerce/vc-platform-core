using System;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.CoreModule.Core.Tax;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.DynamicProperties;
using VirtoCommerce.Platform.Core.Swagger;

namespace VirtoCommerce.OrdersModule.Core.Model
{
    [SwaggerSchemaId("OrderLineItem")]
    public class LineItem : AuditableEntity, IHasTaxDetalization, ISupportCancellation, IHasDimension,
        IHasDynamicProperties, ITaxable, IHasDiscounts, ICloneable
    {
        /// <summary>
        /// Price id
        /// </summary>
        public string PriceId { get; set; }

        public string Currency { get; set; }

        /// <summary>
        ///  unit price without discount and tax
        /// </summary>
        public decimal Price { get; set; }

        public virtual decimal PriceWithTax { get; set; }

        /// <summary>
        /// Resulting price with discount for one unit
        /// </summary>
        public virtual decimal PlacedPrice { get; set; }

        public virtual decimal PlacedPriceWithTax { get; set; }

        public virtual decimal ExtendedPrice { get; set; }

        public virtual decimal ExtendedPriceWithTax { get; set; }

        /// <summary>
        /// Gets the value of the single qty line item discount amount
        /// </summary>
        public virtual decimal DiscountAmount { get; set; }

        public virtual decimal DiscountAmountWithTax { get; set; }

        public decimal DiscountTotal { get; set; }

        public decimal DiscountTotalWithTax { get; set; }

        //Any extra Fee 
        public virtual decimal Fee { get; set; }

        public virtual decimal FeeWithTax { get; set; }

        #region ITaxable Members

        /// <summary>
        /// Tax category or type
        /// </summary>
        public string TaxType { get; set; }

        public decimal TaxTotal { get; set; }

        public decimal TaxPercentRate { get; set; }

        #endregion

        /// <summary>
        /// Reserve quantity
        /// </summary>
        public int ReserveQuantity { get; set; }
        public int Quantity { get; set; }

        public string ProductId { get; set; }
        public string Sku { get; set; }
        public string ProductType { get; set; }
        public string CatalogId { get; set; }
        public string CategoryId { get; set; }

        public string Name { get; set; }

        public string Comment { get; set; }

        public string ImageUrl { get; set; }

        public bool? IsGift { get; set; }
        public string ShippingMethodCode { get; set; }
        public string FulfillmentLocationCode { get; set; }
        public string FulfillmentCenterId { get; set; }
        public string FulfillmentCenterName { get; set; }

        public string OuterId { get; set; }

        #region IHaveDimension Members

        public string WeightUnit { get; set; }
        public decimal? Weight { get; set; }

        public string MeasureUnit { get; set; }
        public decimal? Height { get; set; }
        public decimal? Length { get; set; }
        public decimal? Width { get; set; }

        #endregion

        #region ISupportCancelation Members

        public bool IsCancelled { get; set; }
        public DateTime? CancelledDate { get; set; }
        public string CancelReason { get; set; }

        #endregion

        #region IHasDynamicProperties Members
        public string ObjectType => typeof(LineItem).FullName;
        public ICollection<DynamicObjectProperty> DynamicProperties { get; set; }

        #endregion

        #region IHasDiscounts

        public ICollection<Discount> Discounts { get; set; }

        #endregion

        #region IHaveTaxDetalization Members

        public ICollection<TaxDetail> TaxDetails { get; set; }

        #endregion

        public virtual void ReduceDetails(string responseGroup)
        {
            var orderResponseGroup = EnumUtility.SafeParseFlags(responseGroup, CustomerOrderResponseGroup.Full);

            if (!orderResponseGroup.HasFlag(CustomerOrderResponseGroup.WithDiscounts))
            {
                Discounts = null;
            }
            if (!orderResponseGroup.HasFlag(CustomerOrderResponseGroup.WithPrices))
            {
                Price = 0m;
                PriceWithTax = 0m;
                DiscountAmount = 0m;
                DiscountAmountWithTax = 0m;
                TaxTotal = 0m;
                TaxPercentRate = 0m;
            }
        }

        #region ICloneable members

        public virtual object Clone()
        {
            var result = MemberwiseClone() as LineItem;

            if (DynamicProperties != null)
            {
                result.DynamicProperties = new List<DynamicObjectProperty>(DynamicProperties.Select(x => x.Clone() as DynamicObjectProperty));
            }

            if (Discounts != null)
            {
                result.Discounts = new List<Discount>(Discounts.Select(x => x.Clone() as Discount));
            }

            if (TaxDetails != null)
            {
                result.TaxDetails = new List<TaxDetail>(TaxDetails.Select(x => x.Clone() as TaxDetail));
            }

            return result;
        }

        #endregion
    }
}
