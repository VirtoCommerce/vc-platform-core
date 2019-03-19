using System.Collections.Generic;
using VirtoCommerce.CoreModule.Core.Common;

namespace VirtoCommerce.CoreModule.Core.Conditions
{
    public interface IConditionTree
    {
        ICollection<IConditionTree> AvailableChildren { get; set; }
        ICollection<IConditionTree> Children { get; set; }
        string Id { get; set; }

        bool Evaluate(IEvaluationContext context);
    }
}
