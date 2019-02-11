using coreModel = VirtoCommerce.MarketingModule.Core.Model;
using webModel = VirtoCommerce.MarketingModule.Web.Model;

namespace VirtoCommerce.MarketingModule.Web.Converters
{
    public static class PromotionRewardConverter
    {
        public static webModel.PromotionReward ToWebModel(this coreModel.Promotions.Rewards.PromotionReward reward)
        {
            var retVal = new webModel.PromotionReward();
            //TODO
            retVal.RewardType = reward.GetType().Name;
            return retVal;
        }

    }
}
