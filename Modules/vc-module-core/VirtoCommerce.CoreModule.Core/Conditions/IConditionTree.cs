using System;
using System.Collections.Generic;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CoreModule.Core.Conditions
{
    public interface IConditionTree : IEntity, ICloneable
    {
        IEnumerable<IConditionTree> AvailableChildren { get; }
        IList<IConditionTree> Children { get; set; }

        bool IsSatisfiedBy(IEvaluationContext context);
    }
}
