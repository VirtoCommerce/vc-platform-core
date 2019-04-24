using System.Linq;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.CoreModule.Core.Conditions;

namespace VirtoCommerce.PricingModule.Core.Model.Conditions
{
    public class PriceConditionTree : ConditionTree
    {
        public override bool Evaluate(IEvaluationContext context)
        {
            var result = false;
            if (context is PriceEvaluationContext priceEvaluationContext)
            {
                result = Children.All(c => c.Evaluate(priceEvaluationContext));
            }
            return result;
        }

        public Condition[] GetConditions()
        {
            return Children.OfType<Condition>().ToArray();
        }
    }
}
