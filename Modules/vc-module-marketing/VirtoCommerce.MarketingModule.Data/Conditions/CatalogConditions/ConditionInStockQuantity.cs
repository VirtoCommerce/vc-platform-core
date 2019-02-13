using System;
using VirtoCommerce.Domain.Common;
using VirtoCommerce.Domain.Marketing.Model;
using linq = System.Linq.Expressions;

namespace VirtoCommerce.DynamicExpressionsModule.Data.Promotion
{
    //InStock quantity is []
    public class ConditionInStockQuantity : DynamicExpression, IConditionExpression
    {
        public int Quantity { get; set; }

        public int QuantitySecond { get; set; }

        public string CompareCondition { get; set; }

        public bool Exactly { get; set; }

        public ConditionInStockQuantity()
        {
            CompareCondition = "AtLeast";
        }

        #region IConditionExpression Members
        /// <summary>
        /// ((PromotionEvaluationContext)x).IsItemsInStockQuantity(Exactly, Quantity, QuantitySecond)
        /// </summary>
        /// <returns></returns>
        public linq.Expression<Func<IEvaluationContext, bool>> GetConditionExpression()
        {
            var paramX = linq.Expression.Parameter(typeof(IEvaluationContext), "x");
            var castOp = linq.Expression.MakeUnary(linq.ExpressionType.Convert, paramX, typeof(PromotionEvaluationContext));
            var quantity = linq.Expression.Constant(Quantity);
            var quantitySecond = linq.Expression.Constant(QuantitySecond);
            var methodInfo = typeof(PromotionEvaluationContextExtension).GetMethod("IsItemsInStockQuantityNew");
            var compareCondition = linq.Expression.Constant(CompareCondition);

            var methodCall = linq.Expression.Call(null, methodInfo, castOp, compareCondition, quantity, quantitySecond);

            var retVal = linq.Expression.Lambda<Func<IEvaluationContext, bool>>(methodCall, paramX);

            return retVal;
        }

        #endregion
    }
}
