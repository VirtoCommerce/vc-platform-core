using System.Linq;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.CoreModule.Core.Conditions;

namespace VirtoCommerce.MarketingModule.Core.Model.DynamicContent
{
    public class DynamicContentConditionTree : Condition
    {
        public override bool Evaluate(IEvaluationContext context)
        {
            var result = false;
            if (context is EvaluationContextBase promotionEvaluationContext)
            {
                result = Children.All(c => c.Evaluate(promotionEvaluationContext));
            }
            return result;
        }

        public override Condition[] GetConditions()
        {
            return Children.OfType<Condition>().ToArray();
        }
    }
}
