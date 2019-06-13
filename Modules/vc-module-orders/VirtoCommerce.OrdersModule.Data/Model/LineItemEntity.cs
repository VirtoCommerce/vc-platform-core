using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.CoreModule.Core.Tax;
using VirtoCommerce.OrdersModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.OrdersModule.Data.Model
{
    public class LineItemEntity : AuditableEntity, ISupportPartialPriceUpdate
    {
        [StringLength(128)]
        public string PriceId { get; set; }
        [Required]
        [StringLength(3)]
        public string Currency { get; set; }
        [Column(TypeName = "Money")]
        public decimal? Price { get; set; }
        [Column(TypeName = "Money")]
        public decimal? PriceWithTax { get; set; }
        [Column(TypeName = "Money")]
        public decimal? DiscountAmount { get; set; }
        [Column(TypeName = "Money")]
        public decimal? DiscountAmountWithTax { get; set; }
        [Column(TypeName = "Money")]
        public decimal? TaxTotal { get; set; }
        public decimal? TaxPercentRate { get; set; }
        public int Quantity { get; set; }
        [Required]
        [StringLength(64)]
        public string ProductId { get; set; }
        [Required]
        [StringLength(64)]
        public string CatalogId { get; set; }

        [StringLength(64)]
        public string CategoryId { get; set; }
        [Required]
        [StringLength(64)]
        public string Sku { get; set; }

        [StringLength(64)]
        public string ProductType { get; set; }
        [Required]
        [StringLength(256)]
        public string Name { get; set; }

        [StringLength(2048)]
        public string Comment { get; set; }

        public bool IsReccuring { get; set; }

        [StringLength(1028)]
        public string ImageUrl { get; set; }
        public bool IsGift { get; set; }
        [StringLength(64)]
        public string ShippingMethodCode { get; set; }
        [StringLength(64)]
        public string FulfillmentLocationCode { get; set; }

        [StringLength(32)]
        public string WeightUnit { get; set; }
        public decimal? Weight { get; set; }
        [StringLength(32)]
        public string MeasureUnit { get; set; }
        public decimal? Height { get; set; }
        public decimal? Length { get; set; }
        public decimal? Width { get; set; }

        [StringLength(64)]
        public string TaxType { get; set; }

        public bool IsCancelled { get; set; }
        public DateTime? CancelledDate { get; set; }
        [StringLength(2048)]
        public string CancelReason { get; set; }

        [NotMapped]
        public LineItem ModelLineItem { get; set; }

        public virtual ObservableCollection<DiscountEntity> Discounts { get; set; } = new NullCollection<DiscountEntity>();
        public virtual ObservableCollection<TaxDetailEntity> TaxDetails { get; set; } = new NullCollection<TaxDetailEntity>();

        public virtual CustomerOrderEntity CustomerOrder { get; set; }
        public string CustomerOrderId { get; set; }

        public virtual ObservableCollection<ShipmentItemEntity> ShipmentItems { get; set; } = new NullCollection<ShipmentItemEntity>();

        public virtual LineItem ToModel(LineItem lineItem)
        {
            if (lineItem == null)
                throw new ArgumentNullException(nameof(lineItem));

            lineItem.Id = Id;
            lineItem.CreatedDate = CreatedDate;
            lineItem.CreatedBy = CreatedBy;
            lineItem.ModifiedDate = ModifiedDate;
            lineItem.ModifiedBy = ModifiedBy;

            lineItem.PriceId = PriceId;
            lineItem.CatalogId = CatalogId;
            lineItem.CategoryId = CategoryId;
            lineItem.Currency = Currency;
            lineItem.ProductId = ProductId;
            lineItem.Sku = Sku;
            lineItem.ProductType = ProductType;
            lineItem.Name = Name;
            lineItem.ImageUrl = ImageUrl;
            lineItem.ShippingMethodCode = ShippingMethodCode;
            lineItem.FulfillmentLocationCode = FulfillmentLocationCode;

            lineItem.Price = Price;
            lineItem.PriceWithTax = PriceWithTax;
            lineItem.DiscountAmount = DiscountAmount;
            lineItem.DiscountAmountWithTax = DiscountAmountWithTax;
            lineItem.Quantity = Quantity;
            lineItem.TaxTotal = TaxTotal;
            lineItem.TaxPercentRate = TaxPercentRate;
            lineItem.Weight = Weight;
            lineItem.Height = Height;
            lineItem.Width = Width;
            lineItem.MeasureUnit = MeasureUnit;
            lineItem.WeightUnit = WeightUnit;
            lineItem.Length = Length;
            lineItem.TaxType = TaxType;
            lineItem.IsCancelled = IsCancelled;
            lineItem.CancelledDate = CancelledDate;
            lineItem.CancelReason = CancelReason;
            lineItem.Comment = Comment;
            lineItem.IsGift = IsGift;
            lineItem.Discounts = Discounts.Select(x => x.ToModel(AbstractTypeFactory<Discount>.TryCreateInstance())).ToList();
            lineItem.TaxDetails = TaxDetails.Select(x => x.ToModel(AbstractTypeFactory<TaxDetail>.TryCreateInstance())).ToList();

            return lineItem;
        }

        public virtual LineItemEntity FromModel(LineItem lineItem, PrimaryKeyResolvingMap pkMap)
        {
            if (lineItem == null)
            {
                throw new ArgumentNullException(nameof(lineItem));
            }

            ModelLineItem = lineItem;
            pkMap.AddPair(lineItem, this);

            Id = lineItem.Id;
            CreatedDate = lineItem.CreatedDate;
            CreatedBy = lineItem.CreatedBy;
            ModifiedDate = lineItem.ModifiedDate;
            ModifiedBy = lineItem.ModifiedBy;

            PriceId = lineItem.PriceId;
            CatalogId = lineItem.CatalogId;
            CategoryId = lineItem.CategoryId;
            Currency = lineItem.Currency;
            ProductId = lineItem.ProductId;
            Sku = lineItem.Sku;
            ProductType = lineItem.ProductType;
            Name = lineItem.Name;
            ImageUrl = lineItem.ImageUrl;
            ShippingMethodCode = lineItem.ShippingMethodCode;
            FulfillmentLocationCode = lineItem.FulfillmentLocationCode;

            Price = lineItem.Price;
            PriceWithTax = lineItem.PriceWithTax;
            DiscountAmount = lineItem.DiscountAmount;
            DiscountAmountWithTax = lineItem.DiscountAmountWithTax;
            Quantity = lineItem.Quantity;
            TaxTotal = lineItem.TaxTotal;
            TaxPercentRate = lineItem.TaxPercentRate;
            Weight = lineItem.Weight;
            Height = lineItem.Height;
            Width = lineItem.Width;
            MeasureUnit = lineItem.MeasureUnit;
            WeightUnit = lineItem.WeightUnit;
            Length = lineItem.Length;
            TaxType = lineItem.TaxType;
            IsCancelled = lineItem.IsCancelled;
            CancelledDate = lineItem.CancelledDate;
            CancelReason = lineItem.CancelReason;
            Comment = lineItem.Comment;

            IsGift = lineItem.IsGift ?? false;

            if (lineItem.Discounts != null)
            {
                Discounts = new ObservableCollection<DiscountEntity>();
                Discounts.AddRange(lineItem.Discounts.Select(x => AbstractTypeFactory<DiscountEntity>.TryCreateInstance().FromModel(x)));
            }

            if (lineItem.TaxDetails != null)
            {
                TaxDetails = new ObservableCollection<TaxDetailEntity>();
                TaxDetails.AddRange(lineItem.TaxDetails.Select(x => AbstractTypeFactory<TaxDetailEntity>.TryCreateInstance().FromModel(x)));
            }

            return this;
        }

        public virtual void Patch(LineItemEntity target)
        {
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            target.Quantity = Quantity;
            target.Weight = Weight;
            target.Height = Height;
            target.Width = Width;
            target.MeasureUnit = MeasureUnit;
            target.WeightUnit = WeightUnit;
            target.Length = Length;
            target.TaxType = TaxType;
            target.IsCancelled = IsCancelled;
            target.CancelledDate = CancelledDate;
            target.CancelReason = CancelReason;
            target.Comment = Comment;

            target.TaxPercentRate = TaxPercentRate ?? target.TaxPercentRate;
            target.Price = Price ?? target.Price;
            target.DiscountAmount = DiscountAmount ?? target.DiscountAmount;
            target.PriceWithTax = PriceWithTax ?? target.PriceWithTax;
            target.DiscountAmountWithTax = DiscountAmountWithTax ?? target.DiscountAmountWithTax;
            target.TaxTotal = TaxTotal ?? target.TaxTotal;

            if (!Discounts.IsNullCollection())
            {
                var discountComparer = AnonymousComparer.Create((DiscountEntity x) => x.PromotionId);
                Discounts.Patch(target.Discounts, discountComparer, (sourceDiscount, targetDiscount) => sourceDiscount.Patch(targetDiscount));
            }

            if (!TaxDetails.IsNullCollection())
            {
                var taxDetailComparer = AnonymousComparer.Create((TaxDetailEntity x) => x.Name);
                TaxDetails.Patch(target.TaxDetails, taxDetailComparer, (sourceTaxDetail, targetTaxDetail) => sourceTaxDetail.Patch(targetTaxDetail));
            }
        }
        public virtual void ResetPrices()
        {
            Price = null;
            PriceWithTax = null;
            DiscountAmount = null;
            DiscountAmountWithTax = null;
            TaxTotal = null;
            TaxPercentRate = null;
        }
    }
}
