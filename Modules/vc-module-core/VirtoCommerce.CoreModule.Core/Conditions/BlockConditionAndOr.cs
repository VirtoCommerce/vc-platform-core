using System.Linq;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CoreModule.Core.Conditions
{
    public abstract class BlockConditionAndOr : ConditionTree
    {
        public bool All { get; set; }

        // Logical inverse of expression
        public bool Not { get; set; } = false;


        public override bool IsSatisfiedBy(IEvaluationContext context)
        {
            var result = false;

            if (Children.IsNullOrEmpty())
            {
                return true;
            }

            if (Children != null && Children.Any())
            {
                if (!Not)
                {
                    result = All ? Children.All(ch => ch.IsSatisfiedBy(context)) : Children.Any(ch => ch.IsSatisfiedBy(context));
                }
                else
                {
                    result = All ? !Children.All(ch => ch.IsSatisfiedBy(context)) : !Children.Any(ch => ch.IsSatisfiedBy(context));
                }

            }

            return result;
        }
    }
}
