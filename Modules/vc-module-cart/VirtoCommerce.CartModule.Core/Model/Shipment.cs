using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.CoreModule.Core.Tax;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.DynamicProperties;
using VirtoCommerce.Platform.Core.Swagger;

namespace VirtoCommerce.CartModule.Core.Model
{
    [SwaggerSchemaId("CartShipment")]
    public class Shipment : AuditableEntity, IHasTaxDetalization, ITaxable, IHasDiscounts, IHasDynamicProperties, ICloneable
    {
        public string ShipmentMethodCode { get; set; }
        public string ShipmentMethodOption { get; set; }
        public string FulfillmentCenterId { get; set; }
        public string FulfillmentCenterName { get; set; }
        public string WarehouseLocation { get; set; }

        public string Currency { get; set; }
        public decimal? VolumetricWeight { get; set; }

        public string WeightUnit { get; set; }
        public decimal? Weight { get; set; }

        public string MeasureUnit { get; set; }
        public decimal? Height { get; set; }
        public decimal? Length { get; set; }
        public decimal? Width { get; set; }

        public virtual decimal Price { get; set; }

        public virtual decimal PriceWithTax { get; set; }

        public virtual decimal Total { get; set; }

        public virtual decimal TotalWithTax { get; set; }

        public virtual decimal DiscountAmount { get; set; }
        public virtual decimal DiscountAmountWithTax { get; set; }

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


        public Address DeliveryAddress { get; set; }

        public ICollection<ShipmentItem> Items { get; set; }


        #region IHasDiscounts
        public ICollection<Discount> Discounts { get; set; }
        #endregion

        #region IHaveTaxDetalization Members
        public ICollection<TaxDetail> TaxDetails { get; set; }
        #endregion

        #region IHasDynamicProperties Members
        public string ObjectType => typeof(Shipment).FullName;
        public ICollection<DynamicObjectProperty> DynamicProperties { get; set; }
        #endregion

        #region ICloneable members

        public virtual object Clone()
        {
            var result = MemberwiseClone() as Shipment;

            if (DeliveryAddress != null)
            {
                result.DeliveryAddress = DeliveryAddress.Clone() as Address;
            }

            if (Items != null)
            {
                result.Items = new ObservableCollection<ShipmentItem>(Items.Select(x => x.Clone() as ShipmentItem));
            }

            if (Discounts != null)
            {
                result.Discounts = new ObservableCollection<Discount>(Discounts.Select(x => x.Clone() as Discount));
            }
                        
            if (TaxDetails != null)
            {
                result.TaxDetails = new ObservableCollection<TaxDetail>(TaxDetails.Select(x => x.Clone() as TaxDetail));
            }

            if (DynamicProperties != null)
            {
                result.DynamicProperties = new ObservableCollection<DynamicObjectProperty>(
                    DynamicProperties.Select(x => x.Clone() as DynamicObjectProperty));
            }

            return result;
        }

        #endregion

    }
}
