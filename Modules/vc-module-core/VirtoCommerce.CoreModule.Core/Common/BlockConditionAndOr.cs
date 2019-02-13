using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CoreModule.Core.Common
{
    public abstract class BlockConditionAndOr : ICondition
    {
        public bool All { get; set; }

        // Logical inverse of expression
        public bool Not { get; set; } = false;

        public bool Evaluate(IEvaluationContext context)
        {
            var result = false;
            var expression = All ? PredicateBuilder.True<IEvaluationContext>() : PredicateBuilder.False<IEvaluationContext>();
            var compile = expression.Compile();
            //context.

            return result;
        }
    }
}
