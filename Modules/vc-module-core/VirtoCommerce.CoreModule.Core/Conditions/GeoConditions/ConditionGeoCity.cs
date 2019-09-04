using VirtoCommerce.CoreModule.Core.Common;

namespace VirtoCommerce.CoreModule.Core.Conditions.GeoConditions
{
    //City is []
    public class ConditionGeoCity : MatchedConditionBase
    {
        public override bool IsSatisfiedBy(IEvaluationContext context)
        {
            var result = false;
            if (context is EvaluationContextBase evaluationContext)
            {
                result = UseMatchedCondition(evaluationContext.GeoCity);
            }

            return result;
        }
    }
}
