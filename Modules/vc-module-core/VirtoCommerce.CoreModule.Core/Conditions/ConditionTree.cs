using System.Collections.Generic;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.CoreModule.Core.Common.Conditions;

namespace VirtoCommerce.CoreModule.Core.Conditions
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
