using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CoreModule.Core.Conditions
{
    public abstract class ConditionTree : Entity, IConditionTree
    {
        protected ConditionTree()
        {
            Id = GetType().Name;
        }

        public virtual IEnumerable<IConditionTree> AvailableChildren { get; } =  Array.Empty<IConditionTree>();
        public virtual IList<IConditionTree> Children { get; set; } = new List<IConditionTree>();

        public virtual object Clone()
        {
            var result =  MemberwiseClone() as ConditionTree;
            result.Children = Children?.Select(x => x.Clone() as IConditionTree).ToList();
            return result;
        }

        public virtual bool IsSatisfiedBy(IEvaluationContext context)
        {
            return true;
        }

        #region Conditional JSON serialization for properties declared in base type
        [JsonIgnore]
        public virtual bool ShouldSerializeAvailableChildrenFlag { get; set; }
        /// <summary>
        /// https://www.newtonsoft.com/json/help/html/ConditionalProperties.htm
        /// </summary>
        /// <returns></returns>
        public virtual bool ShouldSerializeAvailableChildren()
        {
            return ShouldSerializeAvailableChildrenFlag;
        }       
        #endregion
    }
}
