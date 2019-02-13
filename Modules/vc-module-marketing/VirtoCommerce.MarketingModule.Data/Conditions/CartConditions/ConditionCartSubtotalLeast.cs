using System;
using System.Collections.Generic;
using System.Globalization;
using VirtoCommerce.Domain.Common;
using VirtoCommerce.Domain.Marketing.Model;
using VirtoCommerce.DynamicExpressionsModule.Data.Common;
using VirtoCommerce.DynamicExpressionsModule.Data.Common.Extensions;
using linq = System.Linq.Expressions;

namespace VirtoCommerce.DynamicExpressionsModule.Data.Promotion
{
    //Cart subtotal is []
    public class ConditionCartSubtotalLeast : CompareConditionBase
    {
        public decimal SubTotal { get; set; }

        public decimal SubTotalSecond { get; set; }


        public ICollection<string> ExcludingCategoryIds { get; set; } = new List<string>();
        public ICollection<string> ExcludingProductIds { get; set; } = new List<string>();


        #region IConditionExpression Members
        /// <summary>
        /// ((PromotionEvaluationContext)x).GetCartTotalWithExcludings(ExcludingCategoryIds, ExcludingProductIds) > SubTotal
        /// </summary>
        /// <returns></returns>
        public override linq.Expression<Func<IEvaluationContext, bool>> GetConditionExpression()
        {
            var paramX = linq.Expression.Parameter(typeof(IEvaluationContext), "x");
            var castOp = linq.Expression.MakeUnary(linq.ExpressionType.Convert, paramX, typeof(PromotionEvaluationContext));
            var subTotal = linq.Expression.Constant(SubTotal);
            var subTotalSecond = linq.Expression.Constant(SubTotalSecond);
            var methodInfo = typeof(PromotionEvaluationContextExtension).GetMethod("GetCartTotalWithExcludings");

            var leftOperandExpression = linq.Expression.Call(null, methodInfo, castOp, ExcludingCategoryIds.GetNewArrayExpression(),
                                                                      ExcludingProductIds.GetNewArrayExpression());
            var rightOperandExpression = linq.Expression.Constant(SubTotal);
            var rightSecondOperandExpression = linq.Expression.Constant(SubTotalSecond);

            var result = linq.Expression.Lambda<Func<IEvaluationContext, bool>>(GetConditionExpression(leftOperandExpression, rightOperandExpression, rightSecondOperandExpression), paramX);
            return result;
        }

        #endregion
    }
}
