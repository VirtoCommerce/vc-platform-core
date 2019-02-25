namespace VirtoCommerce.CoreModule.Core.Common.Conditions
{
    //CUrrent url is []
    public class ConditionUrlIs : MatchedConditionBase
    {
        public override bool Evaluate(IEvaluationContext context)
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
