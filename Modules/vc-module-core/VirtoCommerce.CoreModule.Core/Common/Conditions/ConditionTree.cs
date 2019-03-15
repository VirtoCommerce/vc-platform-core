using System.Collections.Generic;

namespace VirtoCommerce.CoreModule.Core.Common.Conditions
{
    public class ConditionTree : IConditionTree
    {
        public ConditionTree()
        {
            Id = GetType().Name;
            AvailableChildren = new List<IConditionTree>();
            Children = new List<IConditionTree>();
        }

        public ICollection<IConditionTree> AvailableChildren { get; set; }
        public ICollection<IConditionTree> Children { get; set; }
        public string Id { get; set; }
        public virtual bool Evaluate(IEvaluationContext context)
        {
            return true;
        }
    }
}
