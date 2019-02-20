using coreModel = VirtoCommerce.MarketingModule.Core.Model;
using webModel = VirtoCommerce.MarketingModule.Web.Model;

namespace VirtoCommerce.MarketingModule.Web.Converters
{
    public static class PromotionRewardConverter
    {
        public static webModel.PromotionReward ToWebModel(this coreModel.Promotions.PromotionReward reward)
        {
            var retVal = new webModel.PromotionReward
            {
                RewardType = reward.GetType().Name,
                IsValid = reward.IsValid,
                Description = reward.Description,
                CouponAmount = reward.CouponAmount,
                Coupon = reward.Coupon,
                CouponMinOrderAmount = reward.CouponMinOrderAmount,
                Promotion = reward.Promotion,
                PromotionId = reward.Promotion?.Id,
            };
            return retVal;
        }

    }
}
