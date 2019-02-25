namespace VirtoCommerce.CoreModule.Core.Common.Conditions
{
    //Browsing from zip/postal code []
    public class ConditionGeoZipCode : MatchedConditionBase
    {
        public override bool Evaluate(IEvaluationContext context)
        {
            var result = false;
            if (context is EvaluationContextBase evaluationContext)
            {
                result = UseMatchedCondition(evaluationContext.GeoZipCode);
            }

            return result;
        }
    }
}
