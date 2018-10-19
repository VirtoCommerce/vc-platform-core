using System;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.PricingModule.Core.Model;
using linq = System.Linq.Expressions;

namespace VirtoCommerce.PricingModule.Data.DynamicExpressions.Common.Conditions.GeoConditions
{
    //Browsing from a time zone -/+ offset from UTC 
    public class ConditionGeoTimeZone : CompareConditionBase
    {
        public int Value { get; set; }
        public int SecondValue { get; set; }

        public override linq.Expression<Func<IEvaluationContext, bool>> GetConditionExpression()
        {
            linq.ParameterExpression paramX = linq.Expression.Parameter(typeof(IEvaluationContext), "x");
            var castOp = linq.Expression.MakeUnary(linq.ExpressionType.Convert, paramX, typeof(EvaluationContextBase));
            var leftOperandExpression = linq.Expression.Property(castOp, typeof(EvaluationContextBase).GetProperty(ReflectionUtility.GetPropertyName<EvaluationContextBase>(x => x.GeoTimeZone)));

            var rightOperandExpression = linq.Expression.Constant(Value);
            var rightSecondOperandExpression = linq.Expression.Constant(SecondValue);

            var result = linq.Expression.Lambda<Func<IEvaluationContext, bool>>(GetConditionExpression(leftOperandExpression, rightOperandExpression, rightSecondOperandExpression), paramX);
            return result;
        }

    }
}
