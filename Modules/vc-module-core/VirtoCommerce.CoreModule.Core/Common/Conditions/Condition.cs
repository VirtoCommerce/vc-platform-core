namespace VirtoCommerce.CoreModule.Core.Common.Conditions
{
    public class Condition : ConditionTree, ICondition
    {
        public virtual Condition[] GetConditions()
        {
            return new Condition[0];
        }
    }
}
