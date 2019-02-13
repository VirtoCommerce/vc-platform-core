using System;
using System.Collections.Generic;
using VirtoCommerce.Domain.Common;
using VirtoCommerce.Domain.Marketing.Model;
using VirtoCommerce.DynamicExpressionsModule.Data.Common;
using VirtoCommerce.DynamicExpressionsModule.Data.Common.Extensions;
using linq = System.Linq.Expressions;

namespace VirtoCommerce.DynamicExpressionsModule.Data.Promotion
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
