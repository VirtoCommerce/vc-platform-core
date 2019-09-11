using System;
using System.ComponentModel.DataAnnotations;
using VirtoCommerce.MarketingModule.Core.Model.Promotions;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.MarketingModule.Data.Model
{
    public class PromotionUsageEntity : AuditableEntity
    {
        [StringLength(128)]
        public string ObjectId { get; set; }

        [StringLength(128)]
        public string ObjectType { get; set; }

        [StringLength(64)]
        public string CouponCode { get; set; }

        [StringLength(128)]
        public string UserId { get; set; }

        [StringLength(128)]
        public string UserName { get; set; }

        #region Navigation Properties

        public string PromotionId { get; set; }
        public virtual PromotionEntity Promotion { get; set; }

        #endregion

        public virtual PromotionUsage ToModel(PromotionUsage usage)
        {
            if (usage == null)
                throw new NullReferenceException(nameof(usage));

            usage.Id = Id;
            usage.CreatedBy = CreatedBy;
            usage.CreatedDate = CreatedDate;
            usage.ModifiedBy = ModifiedBy;
            usage.ModifiedDate = ModifiedDate;

            usage.CouponCode = CouponCode;
            usage.ObjectId = ObjectId;
            usage.ObjectType = ObjectType;
            usage.PromotionId = PromotionId;
            usage.UserId = UserId;
            usage.UserName = UserName;

            return usage;
        }

        public virtual PromotionUsageEntity FromModel(PromotionUsage usage, PrimaryKeyResolvingMap pkMap)
        {
            if (usage == null)
                throw new NullReferenceException(nameof(usage));

            pkMap.AddPair(usage, this);

            Id = usage.Id;
            CreatedBy = usage.CreatedBy;
            CreatedDate = usage.CreatedDate;
            ModifiedBy = usage.ModifiedBy;
            ModifiedDate = usage.ModifiedDate;

            CouponCode = usage.CouponCode;
            PromotionId = usage.PromotionId;
            ObjectId = usage.ObjectId;
            ObjectType = usage.ObjectType;
            UserId = usage.UserId;
            UserName = usage.UserName;

            return this;
        }

        public virtual void Patch(PromotionUsageEntity target)
        {
            if (target == null)
                throw new NullReferenceException(nameof(target));

            target.ObjectId = ObjectId;
            target.ObjectType = ObjectType;
        }
    }
}
