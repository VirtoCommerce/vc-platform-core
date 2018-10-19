using System;
using System.Reflection;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.DynamicExpressionsModule.Data;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.PricingModule.Core.Model.CommonExpressions;
using linq = System.Linq.Expressions;

namespace VirtoCommerce.PricingModule.Data.DynamicExpressions.Common.Conditions
{
    public abstract class MatchedConditionBase : DynamicExpression, IConditionExpression
    {
        public string Value { get; set; }
        public string MatchCondition { get; set; } = ExpressionConstants.ConditionOperation.Contains;

        #region IConditionExpression Members

        public abstract linq.Expression<Func<IEvaluationContext, bool>> GetConditionExpression();

        #endregion

        public linq.Expression GetConditionExpression(linq.Expression leftOperandExpression)
        {
            MethodInfo method;
            linq.Expression resultExpression = null;

            if (MatchCondition.EqualsInvariant(ExpressionConstants.ConditionOperation.Contains))
            {
                method = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                var toLowerMethod = typeof(string).GetMethod("ToLowerInvariant");
                var toLowerExp = linq.Expression.Call(leftOperandExpression, toLowerMethod);
                resultExpression = linq.Expression.Call(toLowerExp, method, linq.Expression.Constant(Value.ToLowerInvariant()));
            }
            else if (MatchCondition.EqualsInvariant(ExpressionConstants.ConditionOperation.Matching))
            {
                method = typeof(string).GetMethod("Equals", new[] { typeof(string) });
                var toLowerMethod = typeof(string).GetMethod("ToLowerInvariant");
                var toLowerExp = linq.Expression.Call(leftOperandExpression, toLowerMethod);
                resultExpression = linq.Expression.Call(toLowerExp, method, linq.Expression.Constant(Value.ToLowerInvariant()));
            }
            else if (MatchCondition.EqualsInvariant(ExpressionConstants.ConditionOperation.ContainsCase))
            {
                method = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                resultExpression = linq.Expression.Call(leftOperandExpression, method, linq.Expression.Constant(Value));
            }
            else if (MatchCondition.EqualsInvariant(ExpressionConstants.ConditionOperation.MatchingCase))
            {
                method = typeof(string).GetMethod("Equals", new[] { typeof(string) });
                resultExpression = linq.Expression.Call(leftOperandExpression, method, linq.Expression.Constant(Value));
            }
            else if (MatchCondition.EqualsInvariant(ExpressionConstants.ConditionOperation.NotContains))
            {
                method = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                var toLowerMethod = typeof(string).GetMethod("ToLowerInvariant");
                var toLowerExp = linq.Expression.Call(leftOperandExpression, toLowerMethod);
                resultExpression = linq.Expression.Not(linq.Expression.Call(toLowerExp, method, linq.Expression.Constant(Value.ToLowerInvariant())));
            }
            else if (MatchCondition.EqualsInvariant(ExpressionConstants.ConditionOperation.NotMatching))
            {
                method = typeof(string).GetMethod("Equals", new[] { typeof(string) });
                var toLowerMethod = typeof(string).GetMethod("ToLowerInvariant");
                var toLowerExp = linq.Expression.Call(leftOperandExpression, toLowerMethod);
                resultExpression = linq.Expression.Not(linq.Expression.Call(toLowerExp, method, linq.Expression.Constant(Value.ToLowerInvariant())));
            }
            else if (MatchCondition.EqualsInvariant(ExpressionConstants.ConditionOperation.NotContainsCase))
            {
                method = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                resultExpression = linq.Expression.Not(linq.Expression.Call(leftOperandExpression, method, linq.Expression.Constant(Value)));
            }
            else
            {
                method = typeof(string).GetMethod("Equals", new[] { typeof(string) });
                resultExpression = linq.Expression.Not(linq.Expression.Call(leftOperandExpression, method, linq.Expression.Constant(Value)));
            }
            return resultExpression;
        }

    }
}
