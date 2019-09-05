using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.PricingModule.Core.Model;

namespace VirtoCommerce.PricingModule.Data.Model
{
    public class PriceEntity : AuditableEntity, IHasOuterId
    {
        [Column(TypeName = "Money")]
        public decimal? Sale { get; set; }

        [Required]
        [Column(TypeName = "Money")]
        public decimal List { get; set; }

        [StringLength(128)]
        public string ProductId { get; set; }

        [StringLength(1024)]
        public string ProductName { get; set; }

        public decimal MinQuantity { get; set; }

        [StringLength(128)]
        public string OuterId { get; set; }

        #region Navigation Properties

        public string PricelistId { get; set; }
        public virtual PricelistEntity Pricelist { get; set; }

        #endregion

        public virtual Price ToModel(Price price)
        {
            if (price == null)
                throw new ArgumentNullException(nameof(price));

            price.Id = Id;
            price.CreatedBy = CreatedBy;
            price.CreatedDate = CreatedDate;
            price.ModifiedBy = ModifiedBy;
            price.ModifiedDate = ModifiedDate;
            price.OuterId = OuterId;

            price.List = List;
            price.MinQuantity = (int)MinQuantity;
            price.PricelistId = PricelistId;
            price.ProductId = ProductId;
            price.Sale = Sale;

            if (Pricelist != null)
            {
                price.Currency = Pricelist.Currency;
            }

            return price;
        }

        public virtual PriceEntity FromModel(Price price, PrimaryKeyResolvingMap pkMap)
        {
            if (price == null)
                throw new ArgumentNullException(nameof(price));

            pkMap.AddPair(price, this);

            Id = price.Id;
            CreatedBy = price.CreatedBy;
            CreatedDate = price.CreatedDate;
            ModifiedBy = price.ModifiedBy;
            ModifiedDate = price.ModifiedDate;
            OuterId = price.OuterId;

            List = price.List;
            MinQuantity = price.MinQuantity;
            PricelistId = price.PricelistId;
            ProductId = price.ProductId;
            Sale = price.Sale;

            return this;
        }

        public virtual void Patch(PriceEntity target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            target.ProductId = ProductId;
            target.List = List;
            target.Sale = Sale;
            target.MinQuantity = MinQuantity;
        }
    }
}
