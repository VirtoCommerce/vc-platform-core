using VirtoCommerce.PricingModule.Core.Model.Promotions.Rewards;

namespace VirtoCommerce.PricingModule.Core.Model.Promotions.Expressions
{
    public interface IRewardExpression
    {
        PromotionReward[] GetRewards();
    }
}
