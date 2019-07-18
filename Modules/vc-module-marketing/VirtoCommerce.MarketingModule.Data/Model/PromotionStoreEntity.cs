using System;
using System.ComponentModel.DataAnnotations;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.MarketingModule.Data.Model
{
    public class PromotionStoreEntity : Entity, ICloneable
    {
        public string PromotionId { get; set; }

        public virtual PromotionEntity Promotion { get; set; }

        [StringLength(128)]
        [Required]
        public string StoreId { get; set; }

        #region ICloneable members

        public virtual object Clone()
        {
            var result = MemberwiseClone() as PromotionStoreEntity;

            if (Promotion != null)
            {
                result.Promotion = Promotion.Clone() as PromotionEntity;
            }

            return result;
        }

        #endregion
    }
}
