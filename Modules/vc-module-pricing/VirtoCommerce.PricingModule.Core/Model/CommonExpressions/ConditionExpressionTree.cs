using System;
using System.Linq;
using System.Linq.Expressions;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.PricingModule.Core.Model.CommonExpressions
{
    public class ConditionExpressionTree : DynamicExpression, IConditionExpression
    {
        #region IConditionExpression Members

        public virtual Expression<Func<IEvaluationContext, bool>> GetConditionExpression()
        {
            var retVal = PredicateBuilder.True<IEvaluationContext>();
            foreach (var expression in Children.OfType<IConditionExpression>().Select(x => x.GetConditionExpression()).Where(x => x != null))
            {
                retVal = retVal.And(expression);
            }
            return retVal;
        }

        #endregion
    }
}
