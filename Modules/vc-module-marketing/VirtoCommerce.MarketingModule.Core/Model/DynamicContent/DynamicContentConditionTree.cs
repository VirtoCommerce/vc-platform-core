using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.CoreModule.Core.Conditions;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.MarketingModule.Core.Model.DynamicContent
{
    public class DynamicContentConditionTree : ConditionTree
    {
        public override bool IsSatisfiedBy(IEvaluationContext context)
        {
            var result = false;
            if (context is DynamicContentEvaluationContext dynContentEvaluationContext)
            {
                result = Children.All(c => c.IsSatisfiedBy(dynContentEvaluationContext));
            }
            return result;
        }

        public override IEnumerable<IConditionTree> AvailableChildren
        {
            get
            {
                yield return AbstractTypeFactory<BlockContentCondition>.TryCreateInstance();             
            }
        }
    }
}
