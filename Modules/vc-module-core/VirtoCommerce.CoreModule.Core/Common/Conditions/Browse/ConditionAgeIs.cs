namespace VirtoCommerce.CoreModule.Core.Common.Conditions
{
    //Age is []
    public class ConditionAgeIs : CompareConditionBase
    {
        public int Value { get; set; }
        public int SecondValue { get; set; }

        public override bool Evaluate(IEvaluationContext context)
        {
            var result = false;
            if (context is EvaluationContextBase evaluationContext)
            {
                result = UseCompareCondition(evaluationContext.ShopperAge, Value, SecondValue);
            }

            return result;
        }
    }
}
