using System.Collections.Generic;

namespace VirtoCommerce.CoreModule.Core.Common
{
    public class ConditionRewardTree : IConditionRewardTree
    {
        public ConditionRewardTree()
        {
            Id = GetType().Name;
            AvailableChildren = new List<IConditionRewardTree>();
            Children = new List<IConditionRewardTree>();
        }

        public ICollection<IConditionRewardTree> AvailableChildren { get; set; }
        public ICollection<IConditionRewardTree> Children { get; set; }
        public string Id { get; set; }
        public virtual bool Evaluate(IEvaluationContext context)
        {
            return true;
        }
    }
}
