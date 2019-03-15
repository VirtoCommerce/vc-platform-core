using System.Collections.Generic;

namespace VirtoCommerce.CoreModule.Core.Common.Conditions
{
    public interface IConditionTree
    {
        ICollection<IConditionTree> AvailableChildren { get; set; }
        ICollection<IConditionTree> Children { get; set; }
        string Id { get; set; }

        bool Evaluate(IEvaluationContext context);
    }
}
