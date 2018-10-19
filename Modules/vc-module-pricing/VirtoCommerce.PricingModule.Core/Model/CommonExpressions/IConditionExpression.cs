using System;
using VirtoCommerce.CoreModule.Core.Common;

namespace VirtoCommerce.PricingModule.Core.Model.CommonExpressions
{
    public interface IConditionExpression
    {
        System.Linq.Expressions.Expression<Func<IEvaluationContext, bool>> GetConditionExpression();
    }
}
