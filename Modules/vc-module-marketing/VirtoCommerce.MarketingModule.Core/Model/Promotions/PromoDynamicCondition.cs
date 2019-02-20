using System.Linq;
using VirtoCommerce.CoreModule.Core.Common;

namespace VirtoCommerce.MarketingModule.Core.Model.Promotions
{
    public class PromoDynamicCondition : BaseCondition, IRewardExpression
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

        public PromotionReward[] GetRewards()
        {
            var retVal = Children.OfType<IRewardExpression>().SelectMany(x => x.GetRewards()).OfType<PromotionReward>().ToArray();

            return retVal;
        }
    }
}
