using System.Collections.Generic;

namespace VirtoCommerce.CoreModule.Core.Common
{
    public interface IConditionRewardTree
    {
        ICollection<IConditionRewardTree> AvailableChildren { get; set; }
        ICollection<IConditionRewardTree> Children { get; set; }
        string Id { get; set; }

        bool Evaluate(IEvaluationContext context);
    }
}
