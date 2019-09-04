using VirtoCommerce.CoreModule.Core.Common;

namespace VirtoCommerce.CoreModule.Core.Conditions.GeoConditions
{
    //State is []
    public class ConditionGeoState : MatchedConditionBase
    {
        public override bool IsSatisfiedBy(IEvaluationContext context)
        {
            var result = false;
            if (context is EvaluationContextBase evaluationContext)
            {
                result = UseMatchedCondition(evaluationContext.GeoState);
            }

            return result;
        }
    }
}
