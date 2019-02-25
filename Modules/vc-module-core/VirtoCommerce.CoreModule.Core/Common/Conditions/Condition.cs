namespace VirtoCommerce.CoreModule.Core.Common
{
    public class Condition : ConditionRewardTree, ICondition
    {
        public virtual Condition[] GetConditions()
        {
            return new Condition[0];
        }
    }
}
