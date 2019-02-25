using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CoreModule.Core.Common.Conditions
{
    public abstract class MatchedConditionBase : Condition
    {
        public string Value { get; set; }
        public string MatchCondition { get; set; } = ConditionOperation.Contains;

        public virtual bool UseMatchedCondition(string leftOperand)
        {
            var result = false;

            if (MatchCondition.EqualsInvariant(ConditionOperation.Contains))
            {
                result = leftOperand.ToLowerInvariant().Contains(Value.ToLowerInvariant());
            }
            else if (MatchCondition.EqualsInvariant(ConditionOperation.Matching))
            {
                result = leftOperand.ToLowerInvariant().Equals(Value.ToLowerInvariant());
            }
            else if (MatchCondition.EqualsInvariant(ConditionOperation.ContainsCase))
            {
                result = leftOperand.Contains(Value);
            }
            else if (MatchCondition.EqualsInvariant(ConditionOperation.MatchingCase))
            {
                result = leftOperand.Equals(Value);
            }
            else if (MatchCondition.EqualsInvariant(ConditionOperation.NotContains))
            {
                result = !leftOperand.ToLowerInvariant().Contains(Value.ToLowerInvariant());
            }
            else if (MatchCondition.EqualsInvariant(ConditionOperation.NotMatching))
            {
                result = !leftOperand.ToLowerInvariant().Equals(Value.ToLowerInvariant());
            }
            else if (MatchCondition.EqualsInvariant(ConditionOperation.NotContainsCase))
            {
                result = !leftOperand.Contains(Value);
            }
            else
            {
                result = !leftOperand.Equals(Value);
            }

            return result;
        }
    }
}
