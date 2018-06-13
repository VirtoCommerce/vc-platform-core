using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Settings;

namespace VirtoCommerce.StoreModule.Core.Model
{
    public abstract class PaymentMethod : Entity, IHaveSettings, IHaveTaxDetalization, ITaxable
    {
        private PaymentMethod()
        {
            Id = Guid.NewGuid().ToString("N");
        }
        public PaymentMethod(string code)
            : this()
        {
            Code = code;
        }

        /// <summary>
        /// Method identity property (system name)
        /// </summary>
        public string Code { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public string LogoUrl { get; set; }
        public bool IsActive { get; set; }
        public int Priority { get; set; }

        public bool IsAvailableForPartial { get; set; }

        public string Currency { get; set; }

        public virtual decimal Price { get; set; }
        public virtual decimal PriceWithTax
        {
            get
            {
                return Price + Price * TaxPercentRate;
            }
        }

        public virtual decimal Total
        {
            get
            {
                return Price - DiscountAmount;
            }
        }

        public virtual decimal TotalWithTax
        {
            get
            {
                return PriceWithTax - DiscountAmountWithTax;
            }
        }

        public virtual decimal DiscountAmount { get; set; }
        public virtual decimal DiscountAmountWithTax
        {
            get
            {
                return DiscountAmount + DiscountAmount * TaxPercentRate;
            }
        }


        #region IHaveSettings Members

        /// <summary>
        /// Settings of payment method
        /// </summary>
        public ICollection<SettingEntry> Settings { get; set; }
        public virtual string TypeName => GetType().Name;

        #endregion

        #region ITaxable Members

        /// <summary>
        /// Tax category or type
        /// </summary>
        public string TaxType { get; set; }

        public decimal TaxTotal
        {
            get
            {
                return TotalWithTax - Total;
            }
        }

        public decimal TaxPercentRate { get; set; }

        #endregion

        //todo
        //#region ITaxDetailSupport Members

        public ICollection<TaxDetail> TaxDetails { get; set; }

        //#endregion

        /// <summary>
        /// Type of payment method
        /// </summary>
        public abstract PaymentMethodType PaymentMethodType { get; }

        /// <summary>
        /// Type of payment method group
        /// </summary>
        public abstract PaymentMethodGroupType PaymentMethodGroupType { get; }

        //todo
        ///// <summary>
        ///// Method that contains logic of registration payment in external payment system
        ///// </summary>
        ///// <param name="context"></param>
        ///// <returns>Result of registration payment in external payment system</returns>
        //public abstract ProcessPaymentResult ProcessPayment(ProcessPaymentEvaluationContext context);

        ///// <summary>
        ///// Method that contains logic of checking payment status of payment in external payment system
        ///// </summary>
        ///// <param name="context"></param>
        ///// <returns>Result of checking payment in external payment system</returns>
        //public abstract PostProcessPaymentResult PostProcessPayment(PostProcessPaymentEvaluationContext context);

        ///// <summary>
        ///// Voids the payment
        ///// </summary>
        ///// <param name="context"></param>
        ///// <returns>Result of voiding payment in external payment system</returns>
        //public abstract VoidProcessPaymentResult VoidProcessPayment(VoidProcessPaymentEvaluationContext context);

        ///// <summary>
        ///// Capture authorized payment
        ///// </summary>
        ///// <param name="context"></param>
        ///// <returns>Result of capturing payment in external system</returns>
        //public abstract CaptureProcessPaymentResult CaptureProcessPayment(CaptureProcessPaymentEvaluationContext context);

        //public abstract RefundProcessPaymentResult RefundProcessPayment(RefundProcessPaymentEvaluationContext context);

        ///// <summary>
        ///// Method that validates parameters in querystring of request to push URL
        ///// </summary>
        ///// <param name="queryString">Query string of payment push request (external payment system or storefront)</param>
        ///// <returns>Validation result</returns>
        //public abstract ValidatePostProcessRequestResult ValidatePostProcessRequest(NameValueCollection queryString);

        public string GetSetting(string settingName)
        {
            var setting = Settings.FirstOrDefault(s => s.Name == settingName);
            return setting != null ? setting.Value : string.Empty;
        }
    }
}
