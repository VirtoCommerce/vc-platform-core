using System;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.PricingModule.Core.Model.CommonExpressions;
using linq = System.Linq.Expressions;

namespace VirtoCommerce.PricingModule.Data.DynamicExpressions.Common.Conditions
{
    public abstract class CompareConditionBase : DynamicExpression, IConditionExpression
    {

        [Obsolete]
        public bool? Exactly { get; }

        private string _compareCondition = ExpressionConstants.ConditionOperation.AtLeast;

        public virtual string CompareCondition
        {
            get
            {
                //Backward compatibility support
#pragma warning disable 612, 618
                return Exactly.HasValue && Exactly.Value ? ExpressionConstants.ConditionOperation.Exactly : _compareCondition;
#pragma warning restore 612, 618
            }

            set
            {
                _compareCondition = value;
            }
        }

        #region IConditionExpression Members

        public abstract linq.Expression<Func<IEvaluationContext, bool>> GetConditionExpression();

        #endregion

        public linq.BinaryExpression GetConditionExpression(linq.Expression leftOperandExpression, linq.Expression rightOperandExpression, linq.Expression rightSecondOperandExpression = null)
        {
            linq.BinaryExpression binaryOp;

            if (CompareCondition.EqualsInvariant(ExpressionConstants.ConditionOperation.IsMatching) || CompareCondition.EqualsInvariant(ExpressionConstants.ConditionOperation.Exactly))
            {
                binaryOp = linq.Expression.Equal(leftOperandExpression, rightOperandExpression);
            }
            else if (CompareCondition.EqualsInvariant(ExpressionConstants.ConditionOperation.IsNotMatching))
            {
                binaryOp = linq.Expression.NotEqual(leftOperandExpression, rightOperandExpression);
            }
            else if (CompareCondition.EqualsInvariant(ExpressionConstants.ConditionOperation.IsGreaterThan))
            {
                binaryOp = linq.Expression.GreaterThan(leftOperandExpression, rightOperandExpression);
            }
            else if (CompareCondition.EqualsInvariant(ExpressionConstants.ConditionOperation.IsLessThan))
            {
                binaryOp = linq.Expression.LessThan(leftOperandExpression, rightOperandExpression);
            }
            else if (CompareCondition.EqualsInvariant(ExpressionConstants.ConditionOperation.Between))
            {
                binaryOp = linq.Expression.And(linq.Expression.GreaterThanOrEqual(leftOperandExpression, rightOperandExpression),
                    linq.Expression.LessThanOrEqual(leftOperandExpression, rightSecondOperandExpression));
            }
            else if (CompareCondition.EqualsInvariant(ExpressionConstants.ConditionOperation.AtLeast) || CompareCondition.EqualsInvariant(ExpressionConstants.ConditionOperation.IsGreaterThanOrEqual))
            {
                binaryOp = linq.Expression.GreaterThanOrEqual(leftOperandExpression, rightOperandExpression);
            }
            else if (CompareCondition.EqualsInvariant(ExpressionConstants.ConditionOperation.IsLessThanOrEqual))
            {
                binaryOp = linq.Expression.LessThanOrEqual(leftOperandExpression, rightOperandExpression);
            }
            else
                binaryOp = linq.Expression.LessThanOrEqual(leftOperandExpression, rightOperandExpression);

            return binaryOp;
        }

    }
}
