using System;
using System.ComponentModel.DataAnnotations;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.PricingModule.Core.Model;

namespace VirtoCommerce.PricingModule.Data.Model
{
	public class PriceEntity : AuditableEntity
	{
		public decimal? Sale { get; set; }
	
		[Required]
		public decimal List { get; set; }

		[StringLength(128)]
        [Index("ProductIdAndPricelistId", 1)]
        public string ProductId { get; set; }

		[StringLength(1024)]
		public string ProductName { get; set; }

		public decimal MinQuantity { get; set; }

        #region Navigation Properties
        [Index("ProductIdAndPricelistId", 2)]
        public string PricelistId { get; set; }

		public virtual PricelistEntity Pricelist { get; set; }

        #endregion


        public virtual Price ToModel(Price price)
        {
            if (price == null)
                throw new ArgumentNullException(nameof(price));

            price.Id = this.Id;
            price.List = this.List;
            price.MinQuantity =(int)this.MinQuantity;
            price.ModifiedBy = this.ModifiedBy;
            price.ModifiedDate = this.ModifiedDate;
            price.PricelistId = this.PricelistId;
            price.ProductId = this.ProductId;
            price.Sale = this.Sale;

            if (this.Pricelist != null)
            {
                price.Currency = this.Pricelist.Currency;
            }

            return price;
        }

        public virtual PriceEntity FromModel(Price price, PrimaryKeyResolvingMap pkMap)
        {
            if (price == null)
                throw new ArgumentNullException(nameof(price));

            pkMap.AddPair(price, this);

            this.Id = price.Id;
            this.List = price.List;
            this.MinQuantity = price.MinQuantity;
            this.ModifiedBy = price.ModifiedBy;
            this.ModifiedDate = price.ModifiedDate;
            this.PricelistId = price.PricelistId;
            this.ProductId = price.ProductId;
            this.Sale = price.Sale;

            return this;
        }

        public virtual void Patch(PriceEntity target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            target.ProductId = this.ProductId;
            target.List = this.List;
            target.Sale = this.Sale;
            target.MinQuantity = this.MinQuantity;        
        }
    }
}
