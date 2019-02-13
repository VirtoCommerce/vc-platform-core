using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.MarketingModule.Core.Model.Promotions;

namespace VirtoCommerce.MarketingModule.Data.Conditions.CustomerConditions
{
    //Registered user
    public class ConditionIsRegisteredUser : ICondition
    {
        public bool Evaluate(IEvaluationContext context)
        {
            var result = false;
            if (context is PromotionEvaluationContext promotionEvaluationContext)
            {
                result = promotionEvaluationContext.IsRegisteredUser;
            }

            return result;
        }
    }
}
