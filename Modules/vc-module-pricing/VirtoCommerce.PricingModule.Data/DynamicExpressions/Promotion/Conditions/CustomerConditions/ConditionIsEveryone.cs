using System;
using System.Linq;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.PricingModule.Core.Model.CommonExpressions;
using VirtoCommerce.PricingModule.Core.Model.Promotions;
using linq = System.Linq.Expressions;

namespace VirtoCommerce.PricingModule.Data.DynamicExpressions.Promotion.Conditions.CustomerConditions
{
    //Everyone
    public class ConditionIsEveryone : DynamicExpression, IConditionExpression
    {

        #region IConditionExpression Members
        /// <summary>
        /// ((PromotionEvaluationContext)x).IsEveryone
        /// </summary>
        /// <returns></returns>
        public linq.Expression<Func<IEvaluationContext, bool>> GetConditionExpression()
        {
            var paramX = linq.Expression.Parameter(typeof(IEvaluationContext), "x");
            var castOp = linq.Expression.MakeUnary(linq.ExpressionType.Convert, paramX, typeof(PromotionEvaluationContext));
            var memberInfo = typeof(PromotionEvaluationContext).GetMember("IsEveryone").First();
            var isRegistered = linq.Expression.MakeMemberAccess(castOp, memberInfo);

            var retVal = linq.Expression.Lambda<Func<IEvaluationContext, bool>>(isRegistered, paramX);

            return retVal;
        }

        #endregion
    }
}
