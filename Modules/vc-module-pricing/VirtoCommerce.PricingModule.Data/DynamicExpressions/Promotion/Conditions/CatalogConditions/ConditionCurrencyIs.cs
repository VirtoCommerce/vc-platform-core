using System;
using System.Linq;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.PricingModule.Core.Model.CommonExpressions;
using VirtoCommerce.PricingModule.Core.Model.Promotions;
using linq = System.Linq.Expressions;

namespace VirtoCommerce.PricingModule.Data.DynamicExpressions.Promotion.Conditions.CatalogConditions
{
    //Currency is []
    public class ConditionCurrencyIs : DynamicExpression, IConditionExpression
    {
        public string Currency { get; set; }

        #region IConditionExpression Members
        /// <summary>
        /// ((PromotionEvaluationContext)x).Currency == Currency
        /// </summary>
        /// <returns></returns>
        public linq.Expression<Func<IEvaluationContext, bool>> GetConditionExpression()
        {
            var paramX = linq.Expression.Parameter(typeof(IEvaluationContext), "x");
            var castOp = linq.Expression.Convert(paramX, typeof(PromotionEvaluationContext));
            var memberInfo = typeof(PromotionEvaluationContext).GetMember("Currency").First();
            var contextCurrency = linq.Expression.MakeMemberAccess(castOp, memberInfo);
            var selectedCurrency = linq.Expression.Constant(Currency);
            var binaryOp = linq.Expression.Equal(selectedCurrency, contextCurrency);

            var retVal = linq.Expression.Lambda<Func<IEvaluationContext, bool>>(binaryOp, paramX);

            return retVal;
        }

        #endregion
    }
}
