using System;
using System.Collections.Generic;
using VirtoCommerce.CoreModule.Core.Common;

namespace VirtoCommerce.CoreModule.Core.Conditions
{
    public interface IConditionTree : ICloneable
    {
        //Id plays role of type name (discriminator)
        //TODO: rename in future
        string Id { get; }

        /// <summary>
        /// List of all available children for current tree node (is used in expression designer)
        /// </summary>
        IList<IConditionTree> AvailableChildren { get; }
        IList<IConditionTree> Children { get; }

        /// <summary>
        /// Merge tree structure from prototype
        /// </summary>
        /// <param name="prototype"></param>
        void MergeFromPrototype(IConditionTree prototype);

        bool IsSatisfiedBy(IEvaluationContext context);
    }
}
