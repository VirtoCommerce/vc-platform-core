using System.Linq;
using VirtoCommerce.CoreModule.Core.Common;

namespace VirtoCommerce.MarketingModule.Core.Model.Promotions
{
    public class PromotionConditionReward : ConditionRewardTree
    {
        public override bool Evaluate(IEvaluationContext context)
        {
            var result = false;
            if (context is PromotionEvaluationContext promotionEvaluationContext)
            {
                result = Children.All(c => c.Evaluate(promotionEvaluationContext));
            }
            return result;
        }

        public Condition[] GetConditions()
        {
            return Children.OfType<Condition>().ToArray();
        }

        public PromotionReward[] GetRewards()
        {
            return Children.OfType<IReward>().SelectMany(x => x.GetRewards()).ToArray();
        }
    }
}
