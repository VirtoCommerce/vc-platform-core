using VirtoCommerce.CoreModule.Core.Common;

namespace VirtoCommerce.CoreModule.Core.Conditions.Browse
{
    //CUrrent url is []
    public class ConditionUrlIs : MatchedConditionBase
    {
        public override bool IsSatisfiedBy(IEvaluationContext context)
        {
            var result = false;
            if (context is EvaluationContextBase evaluationContext)
            {
                result = UseMatchedCondition(evaluationContext.CurrentUrl);
            }

            return result;
        }
    }
}
