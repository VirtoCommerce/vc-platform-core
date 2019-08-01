using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.CoreModule.Core.Tax;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.DynamicProperties;
using Address = VirtoCommerce.CartModule.Core.Model.Address;

namespace VirtoCommerce.CartModule.Data.Model
{
    public class PaymentEntity : AuditableEntity
    {
        [Required]
        [StringLength(64)]
        public string Currency { get; set; }

        [StringLength(64)]
        public string PaymentGatewayCode { get; set; }

        [Column(TypeName = "Money")]
        public decimal Amount { get; set; }

        [StringLength(1024)]
        public string Purpose { get; set; }

        [StringLength(64)]
        public string TaxType { get; set; }

        [Column(TypeName = "Money")]
        public decimal Price { get; set; }

        [Column(TypeName = "Money")]
        public decimal PriceWithTax { get; set; }

        [Column(TypeName = "Money")]
        public decimal DiscountAmount { get; set; }

        [Column(TypeName = "Money")]
        public decimal DiscountAmountWithTax { get; set; }

        [Column(TypeName = "Money")]
        public decimal Total { get; set; }

        [Column(TypeName = "Money")]
        public decimal TotalWithTax { get; set; }

        [Column(TypeName = "Money")]
        public decimal TaxTotal { get; set; }

        public decimal TaxPercentRate { get; set; }

        public string ShoppingCartId { get; set; }
        public virtual ShoppingCartEntity ShoppingCart { get; set; }

        #region NavigationProperties

        public virtual ObservableCollection<DiscountEntity> Discounts { get; set; }
            = new NullCollection<DiscountEntity>();

        public virtual ObservableCollection<TaxDetailEntity> TaxDetails { get; set; }
            = new NullCollection<TaxDetailEntity>();

        public virtual ObservableCollection<AddressEntity> Addresses { get; set; }
            = new NullCollection<AddressEntity>();

        public virtual ObservableCollection<CartDynamicPropertyObjectValueEntity> DynamicPropertyObjectValues { get; set; }
            = new NullCollection<CartDynamicPropertyObjectValueEntity>();

        #endregion

        public virtual Payment ToModel(Payment payment)
        {
            if (payment == null)
                throw new ArgumentNullException(nameof(payment));

            payment.Id = Id;
            payment.CreatedBy = CreatedBy;
            payment.CreatedDate = CreatedDate;
            payment.ModifiedBy = ModifiedBy;
            payment.ModifiedDate = ModifiedDate;

            payment.Amount = Amount;
            payment.PaymentGatewayCode = PaymentGatewayCode;
            payment.Price = Price;
            payment.PriceWithTax = PriceWithTax;
            payment.DiscountAmount = DiscountAmount;
            payment.DiscountAmountWithTax = DiscountAmountWithTax;
            payment.TaxType = TaxType;
            payment.TaxPercentRate = TaxPercentRate;
            payment.TaxTotal = TaxTotal;
            payment.Total = Total;
            payment.TotalWithTax = TotalWithTax;
            //TODO
            //payment.Purpose = Purpose;
            payment.Currency = Currency;

            if (!TaxDetails.IsNullOrEmpty())
            {
                payment.TaxDetails = TaxDetails.Select(x => x.ToModel(AbstractTypeFactory<TaxDetail>.TryCreateInstance())).ToList();
            }

            if (!Discounts.IsNullOrEmpty())
            {
                payment.Discounts = Discounts.Select(x => x.ToModel(AbstractTypeFactory<Discount>.TryCreateInstance())).ToList();
            }

            if (!Addresses.IsNullOrEmpty())
            {
                payment.BillingAddress = Addresses.First().ToModel(AbstractTypeFactory<Address>.TryCreateInstance());
            }

            payment.DynamicProperties = DynamicPropertyObjectValues.GroupBy(g => g.PropertyId).Select(x =>
            {
                var property = AbstractTypeFactory<DynamicObjectProperty>.TryCreateInstance();
                property.Id = x.Key;
                property.Name = x.FirstOrDefault()?.PropertyName;
                property.Values = x.Select(v => v.ToModel(AbstractTypeFactory<DynamicPropertyObjectValue>.TryCreateInstance())).ToArray();
                return property;
            }).ToArray();

            return payment;
        }

        public virtual PaymentEntity FromModel(Payment payment, PrimaryKeyResolvingMap pkMap)
        {
            if (payment == null)
                throw new ArgumentNullException(nameof(payment));

            pkMap.AddPair(payment, this);

            Id = payment.Id;
            CreatedBy = payment.CreatedBy;
            CreatedDate = payment.CreatedDate;
            ModifiedBy = payment.ModifiedBy;
            ModifiedDate = payment.ModifiedDate;

            Amount = payment.Amount;
            PaymentGatewayCode = payment.PaymentGatewayCode;
            Price = payment.Price;
            PriceWithTax = payment.PriceWithTax;
            DiscountAmount = payment.DiscountAmount;
            DiscountAmountWithTax = payment.DiscountAmountWithTax;
            TaxType = payment.TaxType;
            TaxPercentRate = payment.TaxPercentRate;
            TaxTotal = payment.TaxTotal;
            Total = payment.Total;
            TotalWithTax = payment.TotalWithTax;
            Currency = payment.Currency;
            //TODO
            //Purpose = payment.Purpose;

            if (payment.BillingAddress != null)
            {
                Addresses = new ObservableCollection<AddressEntity>(new[] { AbstractTypeFactory<AddressEntity>.TryCreateInstance().FromModel(payment.BillingAddress) });
            }

            if (payment.TaxDetails != null)
            {
                TaxDetails = new ObservableCollection<TaxDetailEntity>(payment.TaxDetails.Select(x => AbstractTypeFactory<TaxDetailEntity>.TryCreateInstance().FromModel(x)));
            }

            if (payment.Discounts != null)
            {
                Discounts = new ObservableCollection<DiscountEntity>(payment.Discounts.Select(x => AbstractTypeFactory<DiscountEntity>.TryCreateInstance().FromModel(x)));
            }

            if (payment.DynamicProperties != null)
            {
                DynamicPropertyObjectValues = new ObservableCollection<CartDynamicPropertyObjectValueEntity>(payment.DynamicProperties.SelectMany(p => p.Values
                    .Select(v => AbstractTypeFactory<CartDynamicPropertyObjectValueEntity>.TryCreateInstance().FromModel(v, payment, p))).OfType<CartDynamicPropertyObjectValueEntity>());
            }

            return this;
        }

        public virtual void Patch(PaymentEntity target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            target.Amount = Amount;
            target.PaymentGatewayCode = PaymentGatewayCode;
            target.Price = Price;
            target.PriceWithTax = PriceWithTax;
            target.DiscountAmount = DiscountAmount;
            target.DiscountAmountWithTax = DiscountAmountWithTax;
            target.TaxType = TaxType;
            target.TaxPercentRate = TaxPercentRate;
            target.TaxTotal = TaxTotal;
            target.Total = Total;
            target.TotalWithTax = TotalWithTax;
            target.Purpose = Purpose;
            target.Currency = Currency;

            if (!Addresses.IsNullCollection())
            {
                Addresses.Patch(target.Addresses, (sourceAddress, targetAddress) => sourceAddress.Patch(targetAddress));
            }

            if (!TaxDetails.IsNullCollection())
            {
                var taxDetailComparer = AbstractTypeFactory<TaxDetailEntityComparer>.TryCreateInstance();
                TaxDetails.Patch(target.TaxDetails, taxDetailComparer, (sourceTaxDetail, targetTaxDetail) => sourceTaxDetail.Patch(targetTaxDetail));
            }

            if (!Discounts.IsNullCollection())
            {
                var discountComparer = AbstractTypeFactory<DiscountEntityComparer>.TryCreateInstance();
                Discounts.Patch(target.Discounts, discountComparer, (sourceDiscount, targetDiscount) => sourceDiscount.Patch(targetDiscount));
            }

            if (!DynamicPropertyObjectValues.IsNullCollection())
            {
                DynamicPropertyObjectValues.Patch(target.DynamicPropertyObjectValues, (sourceDynamicPropertyObjectValues, targetDynamicPropertyObjectValues) => sourceDynamicPropertyObjectValues.Patch(targetDynamicPropertyObjectValues));
            }
        }
    }
}
