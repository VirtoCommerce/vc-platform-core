using System;
using System.Linq;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.PricingModule.Core.Model.CommonExpressions;

namespace VirtoCommerce.PricingModule.Data.DynamicExpressions
{
    public abstract class BlockConditionAndOr : DynamicExpression, IConditionExpression
    {
        public bool All { get; set; }

        // Logical inverse of expression
        public bool Not { get; set; } = false;

        public virtual System.Linq.Expressions.Expression<Func<IEvaluationContext, bool>> GetConditionExpression()
        {
            System.Linq.Expressions.Expression<Func<IEvaluationContext, bool>> retVal = null;
            if (Children != null && Children.Any())
            {
                retVal = All ? PredicateBuilder.True<IEvaluationContext>() : PredicateBuilder.False<IEvaluationContext>();
                foreach (var expression in Children.OfType<IConditionExpression>().Select(x => x.GetConditionExpression()))
                {
                    retVal = All ? retVal.And(expression) : retVal.Or(expression);
                }
                if (Not)
                {
                    var invokedExpr = System.Linq.Expressions.Expression.Not(System.Linq.Expressions.Expression.Invoke(retVal, retVal.Parameters.Cast<System.Linq.Expressions.Expression>()));
                    retVal = System.Linq.Expressions.Expression.Lambda<Func<IEvaluationContext, bool>>(invokedExpr, retVal.Parameters);
                }
            }
            return retVal;

        }
    }
}
