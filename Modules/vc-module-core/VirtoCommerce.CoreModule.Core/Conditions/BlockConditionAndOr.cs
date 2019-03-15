using System.Linq;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CoreModule.Core.Conditions
{
    public class BlockConditionAndOr : Condition
    {
        public bool All { get; set; }

        // Logical inverse of expression
        public bool Not { get; set; } = false;

        public override bool Evaluate(IEvaluationContext context)
        {
            var result = false;

            if (Children.IsNullOrEmpty() && AvailableChildren.IsNullOrEmpty())
            {
                return true;
            }

            if (Children != null && Children.Any())
            {
                if (!Not)
                {
                    result = All ? Children.All(ch => ch.Evaluate(context)) : Children.Any(ch => ch.Evaluate(context));
                }
                else
                {
                    result = All ? !Children.All(ch => ch.Evaluate(context)) : !Children.Any(ch => ch.Evaluate(context));
                }

            }

            if (AvailableChildren != null && AvailableChildren.Any())
            {
                if (!Not)
                {
                    result = All ? AvailableChildren.All(ch => ch.Evaluate(context)) : AvailableChildren.Any(ch => ch.Evaluate(context));
                }
                else
                {
                    result = All ? !AvailableChildren.All(ch => ch.Evaluate(context)) : !AvailableChildren.Any(ch => ch.Evaluate(context));
                }

            }

            return result;
        }

        public override Condition[] GetConditions()
        {
            return Children.OfType<Condition>().ToArray();
        }
    }
}
