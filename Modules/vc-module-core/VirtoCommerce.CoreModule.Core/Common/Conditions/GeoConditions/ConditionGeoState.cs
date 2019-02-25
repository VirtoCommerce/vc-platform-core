namespace VirtoCommerce.CoreModule.Core.Common.Conditions
{
    //State is []
    public class ConditionGeoState : MatchedConditionBase
    {
        public override bool Evaluate(IEvaluationContext context)
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
