using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using VirtoCommerce.CoreModule.Core.Tax;
using VirtoCommerce.PaymentModule.Model.Requests;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Settings;

namespace VirtoCommerce.PaymentModule.Core.Model
{
    public abstract class PaymentMethod : Entity, IHasSettings, IHasTaxDetalization, ITaxable, ICloneable
    {
        public PaymentMethod(string code)
        {
            Code = code;
        }

        /// <summary>
        /// Method identity property (system name)
        /// </summary>
        public string Code { get; set; }

        public string LogoUrl { get; set; }
        public bool IsActive { get; set; }
        public int Priority { get; set; }

        public bool IsAvailableForPartial { get; set; }

        public string Currency { get; set; }

        public virtual decimal Price { get; set; }
        public virtual decimal PriceWithTax => Price + Price * TaxPercentRate;

        public virtual decimal Total => Price - DiscountAmount;

        public virtual decimal TotalWithTax => PriceWithTax - DiscountAmountWithTax;

        public virtual decimal DiscountAmount { get; set; }
        public virtual decimal DiscountAmountWithTax
        {
            get
            {
                return DiscountAmount + DiscountAmount * TaxPercentRate;
            }
        }

        public string StoreId { get; set; }

        #region IHasSettings Members

        public virtual string TypeName => GetType().Name;

        /// <summary>
        /// Settings of payment method
        /// </summary>
        public ICollection<ObjectSettingEntry> Settings { get; set; }

        #endregion

        #region ITaxable Members

        /// <summary>
        /// Tax category or type
        /// </summary>
        public string TaxType { get; set; }

        public decimal TaxTotal => TotalWithTax - Total;

        public decimal TaxPercentRate { get; set; }

        #endregion

        #region ITaxDetailSupport Members

        public ICollection<TaxDetail> TaxDetails { get; set; }

        #endregion

        /// <summary>
        /// Type of payment method
        /// </summary>
        public abstract PaymentMethodType PaymentMethodType { get; }

        /// <summary>
        /// Type of payment method group
        /// </summary>
        public abstract PaymentMethodGroupType PaymentMethodGroupType { get; }

        /// <summary>
        /// Method that contains logic of registration payment in external payment system
        /// </summary>
        /// <returns>Result of registration payment in external payment system</returns>
        public abstract ProcessPaymentRequestResult ProcessPayment(ProcessPaymentRequest request);

        /// <summary>
        /// Method that contains logic of checking payment status of payment in external payment system
        /// </summary>
        /// <returns>Result of checking payment in external payment system</returns>
        public abstract PostProcessPaymentRequestResult PostProcessPayment(PostProcessPaymentRequest request);

        /// <summary>
        /// Voids the payment
        /// </summary>
        /// <returns>Result of voiding payment in external payment system</returns>
        public abstract VoidPaymentRequestResult VoidProcessPayment(VoidPaymentRequest request);

        /// <summary>
        /// Capture authorized payment
        /// </summary>
        /// <param name="context"></param>
        /// <returns>Result of capturing payment in external system</returns>
        public abstract CapturePaymentRequestResult CaptureProcessPayment(CapturePaymentRequest context);

        public abstract RefundPaymentRequestResult RefundProcessPayment(RefundPaymentRequest context);

        /// <summary>
        /// Method that validates parameters in querystring of request to push URL
        /// </summary>
        /// <param name="queryString">Query string of payment push request (external payment system or storefront)</param>
        /// <returns>Validation result</returns>
        public abstract ValidatePostProcessRequestResult ValidatePostProcessRequest(NameValueCollection queryString);

        #region ICloneable members

        public virtual object Clone()
        {
            var result = MemberwiseClone() as PaymentMethod;

            result.Settings = Settings?.Select(x => x.Clone()).OfType<ObjectSettingEntry>().ToList();
            result.TaxDetails = TaxDetails?.Select(x => x.Clone()).OfType<TaxDetail>().ToList();

            return result;
        }

        #endregion
    }
}
