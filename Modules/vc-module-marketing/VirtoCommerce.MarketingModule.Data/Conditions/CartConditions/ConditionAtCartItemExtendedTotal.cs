using System;
using System.Collections.Generic;
using VirtoCommerce.Domain.Common;
using VirtoCommerce.Domain.Marketing.Model;
using VirtoCommerce.DynamicExpressionsModule.Data.Common;
using VirtoCommerce.DynamicExpressionsModule.Data.Common.Extensions;
using linq = System.Linq.Expressions;

namespace VirtoCommerce.DynamicExpressionsModule.Data.Promotion
{
    //Line item subtotal is []
    public class ConditionAtCartItemExtendedTotal : CompareConditionBase
    {
        public decimal LineItemTotal { get; set; }
        public decimal LineItemTotalSecond { get; set; }

        public ICollection<string> ExcludingCategoryIds { get; set; } = new List<string>();
        public ICollection<string> ExcludingProductIds { get; set; } = new List<string>();


        #region IConditionExpression Members
        /// <summary>
        /// ((PromotionEvaluationContext)x).IsAnyLineItemTotal(LineItemTotal, LineItemTotalSecond, CompareCondition,  ExcludingCategoryIds, ExcludingProductIds)
        /// </summary>
        /// <returns></returns>
        public override linq.Expression<Func<IEvaluationContext, bool>> GetConditionExpression()
        {
            var paramX = linq.Expression.Parameter(typeof(IEvaluationContext), "x");
            var castOp = linq.Expression.MakeUnary(linq.ExpressionType.Convert, paramX, typeof(PromotionEvaluationContext));
            var lineItemTotal = linq.Expression.Constant(LineItemTotal);
            var lineItemTotalSecond = linq.Expression.Constant(LineItemTotalSecond);
            var methodInfo = typeof(PromotionEvaluationContextExtension).GetMethod("IsAnyLineItemExtendedTotalNew");
            var compareCondition = linq.Expression.Constant(CompareCondition);

            var methodCall = linq.Expression.Call(null, methodInfo, castOp, lineItemTotal, lineItemTotalSecond, compareCondition, ExcludingCategoryIds.GetNewArrayExpression(),
                                                                      ExcludingProductIds.GetNewArrayExpression());

            var retVal = linq.Expression.Lambda<Func<IEvaluationContext, bool>>(methodCall, paramX);

            return retVal;
        }

        #endregion
    }
}
