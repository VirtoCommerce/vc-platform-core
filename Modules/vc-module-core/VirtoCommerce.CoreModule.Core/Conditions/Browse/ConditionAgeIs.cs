using VirtoCommerce.CoreModule.Core.Common;

namespace VirtoCommerce.CoreModule.Core.Conditions.Browse
{
    //Age is []
    public class ConditionAgeIs : CompareConditionBase
    {
        public int Value { get; set; }
        public int SecondValue { get; set; }

        public override bool IsSatisfiedBy(IEvaluationContext context)
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
