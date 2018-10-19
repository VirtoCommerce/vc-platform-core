using System;
using System.Collections.Generic;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.PricingModule.Core.Model.Promotions;
using VirtoCommerce.PricingModule.Data.DynamicExpressions.Common.Conditions;
using VirtoCommerce.PricingModule.Data.DynamicExpressions.Common.Extensions;
using VirtoCommerce.PricingModule.Data.DynamicExpressions.Promotion.Helpers;
using linq = System.Linq.Expressions;

namespace VirtoCommerce.PricingModule.Data.DynamicExpressions.Promotion.Conditions.CartConditions
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
