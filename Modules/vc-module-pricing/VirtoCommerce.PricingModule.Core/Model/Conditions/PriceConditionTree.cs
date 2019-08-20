using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.CoreModule.Core.Conditions;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.PricingModule.Core.Model.Conditions
{
    public class PriceConditionTree : ConditionTree
    {
        public override bool IsSatisfiedBy(IEvaluationContext context)
        {
            var result = false;
            if (context is PriceEvaluationContext priceEvaluationContext)
            {
                result = Children.All(c => c.IsSatisfiedBy(priceEvaluationContext));
            }
            return result;
        }

        public override IEnumerable<IConditionTree> AvailableChildren
        {
            get
            {
                yield return AbstractTypeFactory<IConditionTree>.TryCreateInstance(nameof(BlockPricingCondition));
            }
        }
    }
}
