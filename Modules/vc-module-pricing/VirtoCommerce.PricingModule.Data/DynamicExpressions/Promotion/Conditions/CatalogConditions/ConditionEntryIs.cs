using System;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.PricingModule.Core.Model.CommonExpressions;
using VirtoCommerce.PricingModule.Core.Model.Promotions;
using VirtoCommerce.PricingModule.Data.DynamicExpressions.Promotion.Helpers;
using linq = System.Linq.Expressions;
namespace VirtoCommerce.PricingModule.Data.DynamicExpressions.Promotion.Conditions.CatalogConditions
{
    //Product is []
    public class ConditionEntryIs : DynamicExpression, IConditionExpression
    {

        public string ProductId { get; set; }
        public string ProductName { get; set; }

        #region IConditionExpression Members
        /// <summary>
        /// ((PromotionEvaluationContext)x).IsItemInProduct(ProductId)
        /// </summary>
        /// <returns></returns>
        public linq.Expression<Func<IEvaluationContext, bool>> GetConditionExpression()
        {
            var paramX = linq.Expression.Parameter(typeof(IEvaluationContext), "x");
            var castOp = linq.Expression.MakeUnary(linq.ExpressionType.Convert, paramX, typeof(PromotionEvaluationContext));
            var methodInfo = typeof(PromotionEvaluationContextExtension).GetMethod("IsItemInProduct");

            var methodCall = linq.Expression.Call(null, methodInfo, castOp, linq.Expression.Constant(ProductId));
            var retVal = linq.Expression.Lambda<Func<IEvaluationContext, bool>>(methodCall, paramX);

            return retVal;
        }

        #endregion
    }
}
