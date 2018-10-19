using System;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.PricingModule.Core.Model;
using VirtoCommerce.PricingModule.Core.Model.CommonExpressions;
using VirtoCommerce.PricingModule.Data.DynamicExpressions.Common.Helpers;
using linq = System.Linq.Expressions;

namespace VirtoCommerce.PricingModule.Data.DynamicExpressions.Pricing.Conditions
{
    //Tags contains []
    [Obsolete]
    public class TagsContainsCondition : DynamicExpression, IConditionExpression
    {
        public string Tag { get; set; }

        #region IConditionExpression Members
        /// <summary>
        /// ((PriceEvaluationContext)x).TagsContainsKeyword(Keyword)
        /// </summary>
        /// <returns></returns>
        public linq.Expression<Func<IEvaluationContext, bool>> GetConditionExpression()
        {
            var paramX = linq.Expression.Parameter(typeof(IEvaluationContext), "x");
            var castOp = linq.Expression.MakeUnary(linq.ExpressionType.Convert, paramX, typeof(PriceEvaluationContext));
            var methodInfo = typeof(EvaluationContextExtension).GetMethod("UserGroupsContains");

            var methodCall = linq.Expression.Call(null, methodInfo, castOp, linq.Expression.Constant(Tag));

            var retVal = linq.Expression.Lambda<Func<IEvaluationContext, bool>>(methodCall, paramX);

            return retVal;
        }

        #endregion
    }
}
