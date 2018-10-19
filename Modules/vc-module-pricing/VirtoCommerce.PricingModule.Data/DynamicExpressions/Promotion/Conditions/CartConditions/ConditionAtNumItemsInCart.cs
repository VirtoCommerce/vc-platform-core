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
    //[] [] items are in shopping cart
    public class ConditionAtNumItemsInCart : CompareConditionBase
    {
        public ICollection<string> ExcludingCategoryIds { get; set; } = new List<string>();
        public ICollection<string> ExcludingProductIds { get; set; } = new List<string>();

        public int NumItem { get; set; }
        public int NumItemSecond { get; set; }

        #region IConditionExpression Members
        /// <summary>
        /// ((PromotionEvaluationContext)x).GetCartItemsQuantity(ExcludingCategoryIds, ExcludingProductIds) > NumItem
        /// </summary>
        /// <returns></returns>
        public override linq.Expression<Func<IEvaluationContext, bool>> GetConditionExpression()
        {
            var paramX = linq.Expression.Parameter(typeof(IEvaluationContext), "x");
            var castOp = linq.Expression.MakeUnary(linq.ExpressionType.Convert, paramX, typeof(PromotionEvaluationContext));
            var methodInfo = typeof(PromotionEvaluationContextExtension).GetMethod("GetCartItemsQuantity");
            var leftOperandExpression = linq.Expression.Call(null, methodInfo, castOp, ExcludingCategoryIds.GetNewArrayExpression(),
                                                                     ExcludingProductIds.GetNewArrayExpression());
            var rightOperandExpression = linq.Expression.Constant(NumItem);
            var rightSecondOperandExpression = linq.Expression.Constant(NumItemSecond);

            var result = linq.Expression.Lambda<Func<IEvaluationContext, bool>>(GetConditionExpression(leftOperandExpression, rightOperandExpression, rightSecondOperandExpression), paramX);
            return result;
        }

        #endregion
    }
}
