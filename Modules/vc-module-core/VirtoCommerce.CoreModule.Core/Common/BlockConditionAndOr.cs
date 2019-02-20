using System.Linq;

namespace VirtoCommerce.CoreModule.Core.Common
{
    public abstract class BlockConditionAndOr : BaseCondition
    {
        public bool All { get; set; }

        // Logical inverse of expression
        public bool Not { get; set; } = false;

        public override bool Evaluate(IEvaluationContext context)
        {
            var result = false;
            if (Children != null && Children.Any())
            {
                result = All ? Children.All(ch => ch.Evaluate(context)) : Children.Any(ch => ch.Evaluate(context));
            }

            if (AvailableChildren != null && AvailableChildren.Any())
            {
                result = All ? AvailableChildren.All(ch => ch.Evaluate(context)) : AvailableChildren.Any(ch => ch.Evaluate(context));
            }

            return result;
        }
    }
}
