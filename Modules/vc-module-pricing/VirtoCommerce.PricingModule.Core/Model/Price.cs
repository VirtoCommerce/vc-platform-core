using System;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.PricingModule.Core.Model
{
    public class Price : AuditableEntity, ICloneable
    {
        public string PricelistId { get; set; }
        public Pricelist Pricelist { get; set; }
        public string Currency { get; set; }
        public string ProductId { get; set; }
        public decimal? Sale { get; set; }
        public decimal List { get; set; }
        public int MinQuantity { get; set; }

        /// <summary>
        /// Optional start date for this price, so that we can prepare prices ahead of time.
        /// If start date equals now, this price will be active.
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// Optional end date for this price, so that we can prepare prices ahead of time.
        /// If end date equals now, this price will not be active.
        /// </summary>
        public DateTime? EndDate { get; set; }

        public decimal EffectiveValue
        {
            get
            {
                return Sale ?? List;
            }
        }

        public string OuterId { get; set; }

        #region ICloneable Members

        public virtual object Clone()
        {
            var result = MemberwiseClone() as Price;

            result.Pricelist = Pricelist?.Clone() as Pricelist;

            return result;
        }

        #endregion
    }
}
