using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.CoreModule.Core.Tax;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.DynamicProperties;

namespace VirtoCommerce.CartModule.Core.Model
{
    public class Payment : AuditableEntity, IHasTaxDetalization, ITaxable, IHasDiscounts, IHasDynamicProperties, ICloneable
    {
        public string Currency { get; set; }
        public string PaymentGatewayCode { get; set; }
        public decimal Amount { get; set; }

        public Address BillingAddress { get; set; }


        public virtual decimal Price { get; set; }
        public virtual decimal PriceWithTax { get; set; }

        public virtual decimal Total { get; set; }

        public virtual decimal TotalWithTax { get; set; }

        public virtual decimal DiscountAmount { get; set; }
        public virtual decimal DiscountAmountWithTax { get; set; }


        #region ITaxable Members

        /// <summary>
        /// Tax category or type
        /// </summary>
        public string TaxType { get; set; }

        public decimal TaxTotal { get; set; }

        public decimal TaxPercentRate { get; set; }

        #endregion
               
        #region IHasDiscounts
        public ICollection<Discount> Discounts { get; set; }
        #endregion

        #region ITaxDetailSupport Members

        public ICollection<TaxDetail> TaxDetails { get; set; }

        #endregion

        #region IHasDynamicProperties Members
        public string ObjectType => typeof(Payment).FullName;
        public ICollection<DynamicObjectProperty> DynamicProperties { get; set; }
        #endregion

        public object Clone()
        {
            var result = MemberwiseClone() as Payment;

            result.BillingAddress = BillingAddress?.Clone() as Address;
            result.Discounts = Discounts?.Select(x => x.Clone()).OfType<Discount>().ToList();
            result.TaxDetails = TaxDetails?.Select(x => x.Clone()).OfType<TaxDetail>().ToList();
            result.DynamicProperties = DynamicProperties?.Select(x => x.Clone()).OfType<DynamicObjectProperty>().ToList();

            return result;
        }
    }
}
