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
    public class ShoppingCartEntity : AuditableEntity
    {
        [StringLength(64)]
        public string Name { get; set; }

        [Required]
        [StringLength(64)]
        public string StoreId { get; set; }

        [StringLength(64)]
        public string ChannelId { get; set; }

        public bool IsAnonymous { get; set; }

        [Required]
        [StringLength(64)]
        public string CustomerId { get; set; }

        [StringLength(128)]
        public string CustomerName { get; set; }

        [StringLength(64)]
        public string OrganizationId { get; set; }

        [Required]
        [StringLength(3)]
        public string Currency { get; set; }

        [StringLength(16)]
        public string LanguageCode { get; set; }

        public bool TaxIncluded { get; set; }

        public bool IsRecuring { get; set; }

        [StringLength(2048)]
        public string Comment { get; set; }

        [Column(TypeName = "Money")]
        public decimal Total { get; set; }

        [Column(TypeName = "Money")]
        public decimal SubTotal { get; set; }

        [Column(TypeName = "Money")]
        public decimal SubTotalWithTax { get; set; }

        [Column(TypeName = "Money")]
        public decimal ShippingTotal { get; set; }

        [Column(TypeName = "Money")]
        public decimal ShippingTotalWithTax { get; set; }

        [Column(TypeName = "Money")]
        public decimal PaymentTotal { get; set; }

        [Column(TypeName = "Money")]
        public decimal PaymentTotalWithTax { get; set; }

        [Column(TypeName = "Money")]
        public decimal HandlingTotal { get; set; }

        [Column(TypeName = "Money")]
        public decimal HandlingTotalWithTax { get; set; }

        [Column(TypeName = "Money")]
        public decimal DiscountTotal { get; set; }

        [Column(TypeName = "Money")]
        public decimal DiscountTotalWithTax { get; set; }

        [Column(TypeName = "Money")]
        public decimal DiscountAmount { get; set; }

        [Column(TypeName = "Money")]
        public decimal TaxTotal { get; set; }

        [StringLength(64)]
        public string ValidationType { get; set; }

        [StringLength(64)]
        public string Status { get; set; }

        [Column(TypeName = "Money")]
        public decimal Fee { get; set; }

        [Column(TypeName = "Money")]
        public decimal FeeWithTax { get; set; }

        public decimal TaxPercentRate { get; set; }

        [StringLength(64)]
        public string Type { get; set; }

        #region NavigationProperties

        public virtual ObservableCollection<DiscountEntity> Discounts { get; set; } = new NullCollection<DiscountEntity>();
        public virtual ObservableCollection<AddressEntity> Addresses { get; set; } = new NullCollection<AddressEntity>();
        public virtual ObservableCollection<LineItemEntity> Items { get; set; } = new NullCollection<LineItemEntity>();
        public virtual ObservableCollection<PaymentEntity> Payments { get; set; } = new NullCollection<PaymentEntity>();
        public virtual ObservableCollection<ShipmentEntity> Shipments { get; set; } = new NullCollection<ShipmentEntity>();
        public virtual ObservableCollection<TaxDetailEntity> TaxDetails { get; set; } = new NullCollection<TaxDetailEntity>();
        public virtual ObservableCollection<CouponEntity> Coupons { get; set; } = new NullCollection<CouponEntity>();
        public virtual ObservableCollection<CartDynamicPropertyObjectValueEntity> DynamicPropertyObjectValues { get; set; }
            = new NullCollection<CartDynamicPropertyObjectValueEntity>();

        #endregion

        public virtual ShoppingCart ToModel(ShoppingCart cart)
        {
            if (cart == null)
                throw new ArgumentNullException(nameof(cart));

            cart.Id = Id;
            cart.CreatedBy = CreatedBy;
            cart.CreatedDate = CreatedDate;
            cart.ModifiedBy = ModifiedBy;
            cart.ModifiedDate = ModifiedDate;

            cart.StoreId = StoreId;
            cart.Fee = Fee;
            cart.FeeWithTax = FeeWithTax;
            cart.Status = Status;
            cart.Currency = Currency;
            cart.ValidationType = ValidationType;
            cart.CustomerId = CustomerId;
            cart.CustomerName = CustomerName;
            cart.IsAnonymous = IsAnonymous;
            cart.IsRecuring = IsRecuring;
            cart.LanguageCode = LanguageCode;
            cart.Comment = Comment;
            cart.OrganizationId = OrganizationId;
            cart.Total = Total;
            cart.SubTotal = SubTotal;
            cart.SubTotalWithTax = SubTotalWithTax;
            cart.ShippingTotal = ShippingTotal;
            cart.ShippingTotalWithTax = ShippingTotalWithTax;
            cart.PaymentTotal = PaymentTotal;
            cart.PaymentTotalWithTax = PaymentTotalWithTax;
            cart.HandlingTotal = HandlingTotal;
            cart.HandlingTotalWithTax = HandlingTotalWithTax;
            cart.DiscountTotal = DiscountTotal;
            cart.DiscountTotalWithTax = DiscountTotalWithTax;
            cart.DiscountAmount = DiscountAmount;
            cart.TaxTotal = TaxTotal;
            cart.TaxPercentRate = TaxPercentRate;
            cart.Type = Type;
            cart.Name = Name;

            cart.Discounts = Discounts.Select(x => x.ToModel(AbstractTypeFactory<Discount>.TryCreateInstance())).ToList();
            cart.Items = Items.Select(x => x.ToModel(AbstractTypeFactory<LineItem>.TryCreateInstance())).ToList();
            cart.Addresses = Addresses.Select(x => x.ToModel(AbstractTypeFactory<Address>.TryCreateInstance())).ToList();
            cart.Shipments = Shipments.Select(x => x.ToModel(AbstractTypeFactory<Shipment>.TryCreateInstance())).ToList();
            cart.Payments = Payments.Select(x => x.ToModel(AbstractTypeFactory<Payment>.TryCreateInstance())).ToList();
            cart.TaxDetails = TaxDetails.Select(x => x.ToModel(AbstractTypeFactory<TaxDetail>.TryCreateInstance())).ToList();
            cart.Coupons = Coupons.Select(x => x.Code).ToList();

            // Assigning single coupon to preserve backwards compatibility with previous versions of CartModule
            cart.Coupon = cart.Coupons.FirstOrDefault();

            cart.DynamicProperties = DynamicPropertyObjectValues.GroupBy(g => g.PropertyId).Select(x =>
            {
                var property = AbstractTypeFactory<DynamicObjectProperty>.TryCreateInstance();
                property.Id = x.Key;
                property.Name = x.FirstOrDefault()?.PropertyName;
                property.Values = x.Select(v => v.ToModel(AbstractTypeFactory<DynamicPropertyObjectValue>.TryCreateInstance())).ToArray();
                return property;
            }).ToArray();

            return cart;
        }

        public virtual ShoppingCartEntity FromModel(ShoppingCart cart, PrimaryKeyResolvingMap pkMap)
        {
            if (cart == null)
                throw new ArgumentNullException(nameof(cart));

            pkMap.AddPair(cart, this);

            Id = cart.Id;
            CreatedBy = cart.CreatedBy;
            CreatedDate = cart.CreatedDate;
            ModifiedBy = cart.ModifiedBy;
            ModifiedDate = cart.ModifiedDate;

            Fee = cart.Fee;
            FeeWithTax = cart.FeeWithTax;
            Status = cart.Status;
            Currency = cart.Currency;
            ValidationType = cart.ValidationType;
            CustomerId = cart.CustomerId;
            CustomerName = cart.CustomerName;
            IsAnonymous = cart.IsAnonymous;
            IsRecuring = cart.IsRecuring.GetValueOrDefault();
            LanguageCode = cart.LanguageCode;
            Comment = cart.Comment;
            OrganizationId = cart.OrganizationId;
            Total = cart.Total;
            SubTotal = cart.SubTotal;
            SubTotalWithTax = cart.SubTotalWithTax;
            ShippingTotal = cart.ShippingTotal;
            ShippingTotalWithTax = cart.ShippingTotalWithTax;
            PaymentTotal = cart.PaymentTotal;
            PaymentTotalWithTax = cart.PaymentTotalWithTax;
            HandlingTotal = cart.HandlingTotal;
            HandlingTotalWithTax = cart.HandlingTotalWithTax;
            DiscountTotal = cart.DiscountTotal;
            DiscountTotalWithTax = cart.DiscountTotalWithTax;
            DiscountAmount = cart.DiscountAmount;
            TaxTotal = cart.TaxTotal;
            TaxPercentRate = cart.TaxPercentRate;
            Type = cart.Type;
            Name = cart.Name;
            StoreId = cart.StoreId;

            if (cart.Addresses != null)
            {
                Addresses = new ObservableCollection<AddressEntity>(cart.Addresses.Select(x => AbstractTypeFactory<AddressEntity>.TryCreateInstance().FromModel(x)));
            }

            if (cart.Items != null)
            {
                Items = new ObservableCollection<LineItemEntity>(cart.Items.Select(x => AbstractTypeFactory<LineItemEntity>.TryCreateInstance().FromModel(x, pkMap)));
            }

            if (cart.Shipments != null)
            {
                Shipments = new ObservableCollection<ShipmentEntity>(cart.Shipments.Select(x => AbstractTypeFactory<ShipmentEntity>.TryCreateInstance().FromModel(x, pkMap)));
                //Trying to bind shipment items with the  lineItems by model object reference equality
                foreach (var shipmentItemEntity in Shipments.SelectMany(x => x.Items))
                {
                    shipmentItemEntity.LineItem = Items.FirstOrDefault(x => x.ModelLineItem == shipmentItemEntity.ModelLineItem);
                }
            }

            if (cart.Payments != null)
            {
                Payments = new ObservableCollection<PaymentEntity>(cart.Payments.Select(x => AbstractTypeFactory<PaymentEntity>.TryCreateInstance().FromModel(x, pkMap)));
            }

            if (cart.Discounts != null)
            {
                Discounts = new ObservableCollection<DiscountEntity>(cart.Discounts.Select(x => AbstractTypeFactory<DiscountEntity>.TryCreateInstance().FromModel(x)));
            }

            if (cart.TaxDetails != null)
            {
                TaxDetails = new ObservableCollection<TaxDetailEntity>(cart.TaxDetails.Select(x => AbstractTypeFactory<TaxDetailEntity>.TryCreateInstance().FromModel(x)));
            }

            // Extracting single coupon value to preserve backwards compatibility with previous versions of CartModule
            if (cart.Coupon != null)
            {
                Coupons = new ObservableCollection<CouponEntity>(new[] { new CouponEntity { Code = cart.Coupon } });
            }

            if (cart.Coupons != null)
            {
                Coupons = new ObservableCollection<CouponEntity>(cart.Coupons.Select(x => new CouponEntity { Code = x }));
            }

            if (cart.DynamicProperties != null)
            {
                DynamicPropertyObjectValues = new ObservableCollection<CartDynamicPropertyObjectValueEntity>(cart.DynamicProperties.SelectMany(p => p.Values
                    .Select(v => AbstractTypeFactory<CartDynamicPropertyObjectValueEntity>.TryCreateInstance().FromModel(v, cart, p))).OfType<CartDynamicPropertyObjectValueEntity>());
            }

            return this;
        }

        public virtual void Patch(ShoppingCartEntity target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            target.Fee = Fee;
            target.FeeWithTax = FeeWithTax;
            target.Status = Status;
            target.Currency = Currency;
            target.ValidationType = ValidationType;
            target.CustomerId = CustomerId;
            target.CustomerName = CustomerName;
            target.IsAnonymous = IsAnonymous;
            target.IsRecuring = IsRecuring;
            target.LanguageCode = LanguageCode;
            target.Comment = Comment;
            target.OrganizationId = OrganizationId;
            target.Total = Total;
            target.SubTotal = SubTotal;
            target.SubTotalWithTax = SubTotalWithTax;
            target.ShippingTotal = ShippingTotal;
            target.ShippingTotalWithTax = ShippingTotalWithTax;
            target.PaymentTotal = PaymentTotal;
            target.PaymentTotalWithTax = PaymentTotalWithTax;
            target.HandlingTotal = HandlingTotal;
            target.HandlingTotalWithTax = HandlingTotalWithTax;
            target.DiscountTotal = DiscountTotal;
            target.DiscountTotalWithTax = DiscountTotalWithTax;
            target.DiscountAmount = DiscountAmount;
            target.TaxTotal = TaxTotal;
            target.TaxPercentRate = TaxPercentRate;
            target.Type = Type;
            target.Name = Name;

            if (!Items.IsNullCollection())
            {
                Items.Patch(target.Items, (sourceItem, targetItem) => sourceItem.Patch(targetItem));
            }

            if (!Payments.IsNullCollection())
            {
                Payments.Patch(target.Payments, (sourcePayment, targetPayment) => sourcePayment.Patch(targetPayment));
            }

            if (!Addresses.IsNullCollection())
            {
                Addresses.Patch(target.Addresses, (sourceAddress, targetAddress) => sourceAddress.Patch(targetAddress));
            }

            if (!Shipments.IsNullCollection())
            {
                foreach (var shipment in Shipments.Where(x => !x.Items.IsNullCollection()))
                {
                    //Need to remove all items from the shipment with references to non-existing line items.
                    //eft join shipment.Items with cart.Items to detect shipment items are referenced to no longer exist line items
                    var toRemoveItems = shipment.Items.GroupJoin(Items,
                            shipmentItem => shipmentItem.LineItemId ?? shipmentItem.LineItem?.Id,
                            lineItem => lineItem.Id,
                            (shipmentItem, lineItem) => new { ShipmentItem = shipmentItem, LineItem = lineItem.SingleOrDefault() })
                        .Where(x => x.LineItem == null)
                        .Select(x => x.ShipmentItem)
                        .ToArray();
                    foreach (var toRemoveItem in toRemoveItems)
                    {
                        shipment.Items.Remove(toRemoveItem);
                    }
                    //Trying to set appropriator lineItem  from EF dynamic proxy lineItem to avoid EF exception (if shipmentItem.LineItem is new object with Id for already exist LineItem)
                    foreach (var shipmentItem in shipment.Items)
                    {
                        if (shipmentItem.LineItem != null)
                        {
                            shipmentItem.LineItem = target.Items.FirstOrDefault(x => x == shipmentItem.LineItem) ?? shipmentItem.LineItem;
                        }
                    }
                }
                Shipments.Patch(target.Shipments, (sourceShipment, targetShipment) => sourceShipment.Patch(targetShipment));
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

            if (!Coupons.IsNullCollection())
            {
                var couponComparer = AnonymousComparer.Create((CouponEntity x) => x.Code);
                Coupons.Patch(target.Coupons, couponComparer, (sourceCoupon, targetCoupon) => { return; });
            }

            if (!DynamicPropertyObjectValues.IsNullCollection())
            {
                DynamicPropertyObjectValues.Patch(target.DynamicPropertyObjectValues, (sourceDynamicPropertyObjectValues, targetDynamicPropertyObjectValues) => sourceDynamicPropertyObjectValues.Patch(targetDynamicPropertyObjectValues));
            }
        }
    }
}
