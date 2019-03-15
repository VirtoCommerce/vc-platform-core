namespace VirtoCommerce.CoreModule.Core.Common.Conditions
{
    //Language is []
    public class ConditionLanguageIs : MatchedConditionBase
    {
        public override bool Evaluate(IEvaluationContext context)
        {
            var result = false;
            if (context is EvaluationContextBase evaluationContext)
            {
                result = UseMatchedCondition(evaluationContext.Language);
            }

            return result;
        }
    }
}
