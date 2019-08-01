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

namespace VirtoCommerce.CartModule.Data.Model
{
    public class LineItemEntity : AuditableEntity
    {
        [Required]
        [StringLength(3)]
        public string Currency { get; set; }

        [Required]
        [StringLength(64)]
        public string ProductId { get; set; }

        [Required]
        [StringLength(64)]
        public string Sku { get; set; }

        [Required]
        [StringLength(64)]
        public string CatalogId { get; set; }

        [StringLength(64)]
        public string CategoryId { get; set; }

        [StringLength(64)]
        public string ProductType { get; set; }

        [Required]
        [StringLength(256)]
        public string Name { get; set; }

        public int Quantity { get; set; }

        [StringLength(64)]
        public string FulfillmentLocationCode { get; set; }

        [StringLength(64)]
        public string ShipmentMethodCode { get; set; }

        public bool RequiredShipping { get; set; }

        [StringLength(1028)]
        public string ImageUrl { get; set; }

        public bool IsGift { get; set; }

        [StringLength(16)]
        public string LanguageCode { get; set; }

        [StringLength(2048)]
        public string Comment { get; set; }

        [StringLength(64)]
        public string ValidationType { get; set; }

        public bool IsReccuring { get; set; }

        public bool TaxIncluded { get; set; }

        public decimal? VolumetricWeight { get; set; }

        [StringLength(32)]
        public string WeightUnit { get; set; }

        public decimal? Weight { get; set; }

        [StringLength(32)]
        public string MeasureUnit { get; set; }

        public decimal? Height { get; set; }

        public decimal? Length { get; set; }

        public decimal? Width { get; set; }

        public bool IsReadOnly { get; set; }

        [StringLength(128)]
        public string PriceId { get; set; }

        [Column(TypeName = "Money")]
        public decimal ListPrice { get; set; }

        [Column(TypeName = "Money")]
        public decimal ListPriceWithTax { get; set; }

        [Column(TypeName = "Money")]
        public decimal SalePrice { get; set; }

        [Column(TypeName = "Money")]
        public decimal SalePriceWithTax { get; set; }

        [Column(TypeName = "Money")]
        public decimal DiscountAmount { get; set; }

        [Column(TypeName = "Money")]
        public decimal DiscountAmountWithTax { get; set; }

        [Column(TypeName = "Money")]
        public decimal Fee { get; set; }

        [Column(TypeName = "Money")]
        public decimal FeeWithTax { get; set; }

        [Column(TypeName = "Money")]
        public decimal TaxTotal { get; set; }

        public decimal TaxPercentRate { get; set; }

        [StringLength(64)]
        public string TaxType { get; set; }

        [NotMapped]
        public LineItem ModelLineItem { get; set; }

        public string ShoppingCartId { get; set; }
        public virtual ShoppingCartEntity ShoppingCart { get; set; }

        #region NavigationProperties

        public virtual ObservableCollection<TaxDetailEntity> TaxDetails { get; set; }
            = new NullCollection<TaxDetailEntity>();

        public virtual ObservableCollection<DiscountEntity> Discounts { get; set; }
            = new NullCollection<DiscountEntity>();

        public virtual ObservableCollection<CartDynamicPropertyObjectValueEntity> DynamicPropertyObjectValues { get; set; }
            = new NullCollection<CartDynamicPropertyObjectValueEntity>();

        #endregion

        public virtual LineItem ToModel(LineItem lineItem)
        {
            if (lineItem == null)
                throw new ArgumentNullException(nameof(lineItem));

            lineItem.Id = Id;
            lineItem.CreatedBy = CreatedBy;
            lineItem.CreatedDate = CreatedDate;
            lineItem.ModifiedBy = ModifiedBy;
            lineItem.ModifiedDate = ModifiedDate;

            lineItem.ListPrice = ListPrice;
            lineItem.ListPriceWithTax = ListPriceWithTax;
            lineItem.SalePrice = SalePrice;
            lineItem.SalePriceWithTax = SalePriceWithTax;
            lineItem.Fee = Fee;
            lineItem.FeeWithTax = FeeWithTax;
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
            lineItem.IsReadOnly = IsReadOnly;
            lineItem.ValidationType = ValidationType;
            lineItem.PriceId = PriceId;
            lineItem.LanguageCode = LanguageCode;
            lineItem.IsReccuring = IsReccuring;
            lineItem.IsGift = IsGift;
            lineItem.ImageUrl = ImageUrl;
            lineItem.ProductId = ProductId;
            lineItem.ProductType = ProductType;
            lineItem.ShipmentMethodCode = ShipmentMethodCode;
            lineItem.RequiredShipping = RequiredShipping;
            lineItem.ProductType = ProductType;
            lineItem.FulfillmentLocationCode = FulfillmentLocationCode;
            lineItem.Note = Comment;
            lineItem.CatalogId = CatalogId;
            lineItem.CategoryId = CategoryId;
            lineItem.Currency = Currency;
            lineItem.Name = Name;
            lineItem.Sku = Sku;

            if (!Discounts.IsNullOrEmpty())
            {
                lineItem.Discounts = Discounts.Select(x => x.ToModel(AbstractTypeFactory<Discount>.TryCreateInstance())).ToList();
            }

            if (!TaxDetails.IsNullOrEmpty())
            {
                lineItem.TaxDetails = TaxDetails.Select(x => x.ToModel(AbstractTypeFactory<TaxDetail>.TryCreateInstance())).ToList();
            }

            lineItem.DynamicProperties = DynamicPropertyObjectValues.GroupBy(g => g.PropertyId).Select(x =>
            {
                var property = AbstractTypeFactory<DynamicObjectProperty>.TryCreateInstance();
                property.Id = x.Key;
                property.Name = x.FirstOrDefault()?.PropertyName;
                property.Values = x.Select(v => v.ToModel(AbstractTypeFactory<DynamicPropertyObjectValue>.TryCreateInstance())).ToArray();
                return property;
            }).ToArray();

            return lineItem;
        }

        public virtual LineItemEntity FromModel(LineItem lineItem, PrimaryKeyResolvingMap pkMap)
        {
            if (lineItem == null)
                throw new ArgumentNullException(nameof(lineItem));

            pkMap.AddPair(lineItem, this);

            Id = lineItem.Id;
            CreatedBy = lineItem.CreatedBy;
            CreatedDate = lineItem.CreatedDate;
            ModifiedBy = lineItem.ModifiedBy;
            ModifiedDate = lineItem.ModifiedDate;

            ListPrice = lineItem.ListPrice;
            ListPriceWithTax = lineItem.ListPriceWithTax;
            SalePrice = lineItem.SalePrice;
            SalePriceWithTax = lineItem.SalePriceWithTax;
            Fee = lineItem.Fee;
            FeeWithTax = lineItem.FeeWithTax;
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
            IsReadOnly = lineItem.IsReadOnly;
            ValidationType = lineItem.ValidationType;
            PriceId = lineItem.PriceId;
            LanguageCode = lineItem.LanguageCode;
            IsReccuring = lineItem.IsReccuring;
            IsGift = lineItem.IsGift;
            ImageUrl = lineItem.ImageUrl;
            ProductId = lineItem.ProductId;
            ProductType = lineItem.ProductType;
            ShipmentMethodCode = lineItem.ShipmentMethodCode;
            RequiredShipping = lineItem.RequiredShipping;
            ProductType = lineItem.ProductType;
            FulfillmentLocationCode = lineItem.FulfillmentLocationCode;
            Comment = lineItem.Note;
            CatalogId = lineItem.CatalogId;
            CategoryId = lineItem.CategoryId;
            Currency = lineItem.Currency;
            Name = lineItem.Name;
            Sku = lineItem.Sku;
            //Preserve link of the  original model LineItem for future references binding LineItems with  ShipmentLineItems 
            ModelLineItem = lineItem;

            if (lineItem.Discounts != null)
            {
                Discounts = new ObservableCollection<DiscountEntity>(lineItem.Discounts.Select(x => AbstractTypeFactory<DiscountEntity>.TryCreateInstance().FromModel(x)));
            }

            if (lineItem.TaxDetails != null)
            {
                TaxDetails = new ObservableCollection<TaxDetailEntity>(lineItem.TaxDetails.Select(x => AbstractTypeFactory<TaxDetailEntity>.TryCreateInstance().FromModel(x)));
            }

            if (lineItem.DynamicProperties != null)
            {
                DynamicPropertyObjectValues = new ObservableCollection<CartDynamicPropertyObjectValueEntity>(lineItem.DynamicProperties.SelectMany(p => p.Values
                    .Select(v => AbstractTypeFactory<CartDynamicPropertyObjectValueEntity>.TryCreateInstance().FromModel(v, lineItem, p))).OfType<CartDynamicPropertyObjectValueEntity>());
            }

            return this;
        }

        public virtual void Patch(LineItemEntity target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            target.Name = Name;
            target.ListPrice = ListPrice;
            target.ListPriceWithTax = ListPriceWithTax;
            target.SalePrice = SalePrice;
            target.SalePriceWithTax = SalePriceWithTax;
            target.Fee = Fee;
            target.FeeWithTax = FeeWithTax;
            target.DiscountAmount = DiscountAmount;
            target.DiscountAmountWithTax = DiscountAmountWithTax;
            target.Quantity = Quantity;
            target.TaxTotal = TaxTotal;
            target.TaxPercentRate = TaxPercentRate;
            target.Weight = Weight;
            target.Height = Height;
            target.Width = Width;
            target.MeasureUnit = MeasureUnit;
            target.WeightUnit = WeightUnit;
            target.Length = Length;
            target.TaxType = TaxType;
            target.Comment = Comment;
            target.IsReadOnly = IsReadOnly;
            target.ValidationType = ValidationType;
            target.PriceId = PriceId;
            target.LanguageCode = LanguageCode;
            target.IsReccuring = IsReccuring;
            target.IsGift = IsGift;
            target.ImageUrl = ImageUrl;
            target.ProductId = ProductId;
            target.ProductType = ProductType;
            target.ShipmentMethodCode = ShipmentMethodCode;
            target.RequiredShipping = RequiredShipping;
            target.ProductType = ProductType;
            target.FulfillmentLocationCode = FulfillmentLocationCode;
            target.Sku = Sku;
            if (!Discounts.IsNullCollection())
            {
                var discountComparer = AbstractTypeFactory<DiscountEntityComparer>.TryCreateInstance();
                Discounts.Patch(target.Discounts, discountComparer, (sourceDiscount, targetDiscount) => sourceDiscount.Patch(targetDiscount));
            }

            if (!TaxDetails.IsNullCollection())
            {
                var taxDetailComparer = AbstractTypeFactory<TaxDetailEntityComparer>.TryCreateInstance();
                TaxDetails.Patch(target.TaxDetails, taxDetailComparer, (sourceTaxDetail, targetTaxDetail) => sourceTaxDetail.Patch(targetTaxDetail));
            }

            if (!DynamicPropertyObjectValues.IsNullCollection())
            {
                DynamicPropertyObjectValues.Patch(target.DynamicPropertyObjectValues, (sourceDynamicPropertyObjectValues, targetDynamicPropertyObjectValues) => sourceDynamicPropertyObjectValues.Patch(targetDynamicPropertyObjectValues));
            }
        }
    }
}
