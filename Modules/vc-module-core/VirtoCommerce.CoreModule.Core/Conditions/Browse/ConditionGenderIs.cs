using VirtoCommerce.CoreModule.Core.Common;

namespace VirtoCommerce.CoreModule.Core.Conditions.Browse
{
    //Shopper gender is []
    public class ConditionGenderIs : MatchedConditionBase
    {
        public ConditionGenderIs()
        {
            Value = "female";
        }

        public override bool Evaluate(IEvaluationContext context)
        {
            var result = false;
            if (context is EvaluationContextBase evaluationContext)
            {
                result = UseMatchedCondition(evaluationContext.ShopperGender);
            }

            return result;
        }
    }
}
