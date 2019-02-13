using System;
using VirtoCommerce.Domain.Common;
using VirtoCommerce.Domain.Marketing.Model;
using VirtoCommerce.DynamicExpressionsModule.Data.Common;
using linq = System.Linq.Expressions;

namespace VirtoCommerce.DynamicExpressionsModule.Data.Promotion
{
    //[] [] items of entry are in shopping cart
    public class ConditionAtNumItemsOfEntryAreInCart : CompareConditionBase
    {

        public int NumItem { get; set; }
        public int NumItemSecond { get; set; }

        public string ProductId { get; set; }
        public string ProductName { get; set; }


        #region IConditionExpression Members
        /// <summary>
        /// ((PromotionEvaluationContext)x).GetCartItemsOfProductQuantity(ProductId, ExcludingCategoryIds, ExcludingProductIds) > NumItem
        /// </summary>
        /// <returns></returns>
        public override linq.Expression<Func<IEvaluationContext, bool>> GetConditionExpression()
        {
            var paramX = linq.Expression.Parameter(typeof(IEvaluationContext), "x");
            var castOp = linq.Expression.MakeUnary(linq.ExpressionType.Convert, paramX, typeof(PromotionEvaluationContext));
            var methodInfo = typeof(PromotionEvaluationContextExtension).GetMethod("GetCartItemsOfProductQuantity");
            var leftOperandExpression = linq.Expression.Call(null, methodInfo, castOp, linq.Expression.Constant(ProductId));
            var rightOperandExpression = linq.Expression.Constant(NumItem);
            var rightSecondOperandExpression = linq.Expression.Constant(NumItemSecond);

            var result = linq.Expression.Lambda<Func<IEvaluationContext, bool>>(GetConditionExpression(leftOperandExpression, rightOperandExpression, rightSecondOperandExpression), paramX);
            return result;
        }

        #endregion
    }
}
