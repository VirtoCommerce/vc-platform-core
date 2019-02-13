using System;
using System.Collections.Generic;
using VirtoCommerce.Domain.Common;
using VirtoCommerce.Domain.Marketing.Model;
using VirtoCommerce.DynamicExpressionsModule.Data.Common.Extensions;
using linq = System.Linq.Expressions;

namespace VirtoCommerce.DynamicExpressionsModule.Data.Promotion
{
    //Category is []
    public class ConditionCategoryIs : DynamicExpression, IConditionExpression
    {
        public ICollection<string> ExcludingCategoryIds { get; set; } = new List<string>();
        public ICollection<string> ExcludingProductIds { get; set; } = new List<string>();

        public string CategoryId { get; set; }
        public string CategoryName { get; set; }

        #region IConditionExpression Members
        /// <summary>
        /// ((PromotionEvaluationContext)x).IsItemInCategory(CategoryId, ExcludingCategoryIds, ExcludingProductIds)
        /// </summary>
        /// <returns></returns>
        public linq.Expression<Func<IEvaluationContext, bool>> GetConditionExpression()
        {
            var paramX = linq.Expression.Parameter(typeof(IEvaluationContext), "x");
            var castOp = linq.Expression.MakeUnary(linq.ExpressionType.Convert, paramX, typeof(PromotionEvaluationContext));
            var methodInfo = typeof(PromotionEvaluationContextExtension).GetMethod("IsItemInCategory");

            var methodCall = linq.Expression.Call(null, methodInfo, castOp, linq.Expression.Constant(CategoryId), ExcludingCategoryIds.GetNewArrayExpression(),
                                                  ExcludingProductIds.GetNewArrayExpression());
            var retVal = linq.Expression.Lambda<Func<IEvaluationContext, bool>>(methodCall, paramX);

            return retVal;

        }

        #endregion
    }
}
