using System.Collections.Generic;
using System.Linq;
using linq = System.Linq.Expressions;

namespace VirtoCommerce.PricingModule.Data.DynamicExpressions.Common.Extensions
{
    public static class ExpressionExtensions
    {
        public static linq.NewArrayExpression GetNewArrayExpression(this IEnumerable<string> items)
        {
            var trees = new List<linq.Expression>();
            trees.AddRange(items.Select(linq.Expression.Constant));
            return linq.Expression.NewArrayInit(typeof(string), trees);
        }

    }
}
