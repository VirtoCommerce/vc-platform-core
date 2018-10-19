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
    //[] [] items of category are in shopping cart
    public class ConditionAtNumItemsInCategoryAreInCart : CompareConditionBase
    {
        public int NumItem { get; set; }
        public int NumItemSecond { get; set; }

        public ICollection<string> ExcludingCategoryIds { get; set; } = new List<string>();
        public ICollection<string> ExcludingProductIds { get; set; } = new List<string>();

        public string CategoryId { get; set; }

        public string CategoryName { get; set; }

        #region IConditionExpression Members
        /// <summary>
        /// ((PromotionEvaluationContext)x).GetCartItemsOfCategoryQuantity(CategoryId, ExcludingCategoryIds, ExcludingProductIds) > NumItem
        /// </summary>
        /// <returns></returns>
        public override linq.Expression<Func<IEvaluationContext, bool>> GetConditionExpression()
        {
            var paramX = linq.Expression.Parameter(typeof(IEvaluationContext), "x");
            var castOp = linq.Expression.MakeUnary(linq.ExpressionType.Convert, paramX, typeof(PromotionEvaluationContext));
            var methodInfo = typeof(PromotionEvaluationContextExtension).GetMethod("GetCartItemsOfCategoryQuantity");
            var leftOperandExpression = linq.Expression.Call(null, methodInfo, castOp, linq.Expression.Constant(CategoryId), ExcludingCategoryIds.GetNewArrayExpression(),
                                                                      ExcludingProductIds.GetNewArrayExpression());
            var rightOperandExpression = linq.Expression.Constant(NumItem);
            var rightSecondOperandExpression = linq.Expression.Constant(NumItemSecond);

            var result = linq.Expression.Lambda<Func<IEvaluationContext, bool>>(GetConditionExpression(rightOperandExpression, rightOperandExpression, rightSecondOperandExpression), paramX);
            return result;
        }

        #endregion
    }
}
