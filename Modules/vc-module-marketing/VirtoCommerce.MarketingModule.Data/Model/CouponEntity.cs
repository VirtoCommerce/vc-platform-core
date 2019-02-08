using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VirtoCommerce.MarketingModule.Core.Model.Promotions;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.MarketingModule.Data.Model
{
    public class CouponEntity : AuditableEntity
    {
        public CouponEntity()
        {
        }

        [StringLength(64)]
        //TODO
        //[Index("IX_CodeAndPromotionId", IsUnique = true, Order = 1)]
        public string Code { get; set; }

        public int MaxUsesNumber { get; set; }

        public int MaxUsesPerUser { get; set; }

        public DateTime? ExpirationDate { get; set; }

        [NotMapped]
        public long TotalUsesCount { get; set; }

        #region Navigation Properties
        //TODO
        //[Index("IX_CodeAndPromotionId", IsUnique = true, Order = 2)]
        public string PromotionId { get; set; }
        public virtual PromotionEntity Promotion { get; set; }

        #endregion

        public virtual Coupon ToModel(Coupon coupon)
        {
            if (coupon == null)
                throw new NullReferenceException(nameof(coupon));

            coupon.Code = Code;
            coupon.CreatedBy = CreatedBy;
            coupon.CreatedDate = CreatedDate;
            coupon.ExpirationDate = ExpirationDate;
            coupon.Id = Id;
            coupon.MaxUsesNumber = MaxUsesNumber;
            coupon.ModifiedBy = ModifiedBy;
            coupon.ModifiedDate = ModifiedDate;
            coupon.MaxUsesNumber = MaxUsesNumber;
            coupon.PromotionId = PromotionId;
            coupon.TotalUsesCount = TotalUsesCount;
            coupon.MaxUsesPerUser = MaxUsesPerUser;

            return coupon;
        }

        public virtual CouponEntity FromModel(Coupon coupon, PrimaryKeyResolvingMap pkMap)
        {
            if (coupon == null)
                throw new NullReferenceException(nameof(coupon));

            pkMap.AddPair(coupon, this);

            Code = coupon.Code;
            CreatedBy = coupon.CreatedBy;
            CreatedDate = coupon.CreatedDate;
            ExpirationDate = coupon.ExpirationDate;
            Id = coupon.Id;
            MaxUsesNumber = coupon.MaxUsesNumber;
            MaxUsesPerUser = coupon.MaxUsesPerUser;
            ModifiedBy = coupon.ModifiedBy;
            ModifiedDate = coupon.ModifiedDate;
            MaxUsesNumber = coupon.MaxUsesNumber;
            PromotionId = coupon.PromotionId;
            TotalUsesCount = coupon.TotalUsesCount;
            return this;
        }

        public virtual void Patch(CouponEntity target)
        {
            if (target == null)
                throw new NullReferenceException(nameof(target));

            target.Code = Code;
            target.ExpirationDate = ExpirationDate;
            target.MaxUsesNumber = MaxUsesNumber;
            target.MaxUsesPerUser = MaxUsesPerUser;
        }
    }
}
