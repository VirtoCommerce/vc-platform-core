using System;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CoreModule.Core.Conditions
{
    public abstract class ConditionTree : ValueObject, IConditionTree
    {
        protected ConditionTree()
        {
            Id = GetType().Name;
        }

        //Id plays role of type name (discriminator)
        //TODO: rename in future to something else
        public string Id { get; protected set; }

        public virtual IList<IConditionTree> AvailableChildren { get; set; } = new List<IConditionTree>();
        public virtual IList<IConditionTree> Children { get; set; } = new List<IConditionTree>();

        public ConditionTree WithAvailConditions(params IConditionTree[]  availConditions)
        {
            if (availConditions == null)
            {
                throw new ArgumentNullException(nameof(availConditions));
            }
            AvailableChildren.AddRange(availConditions);
            return this;
        }

        public ConditionTree WithChildrens(params IConditionTree[] childrenCondition)
        {
            if (childrenCondition == null)
            {
                throw new ArgumentNullException(nameof(childrenCondition));
            }
            Children.AddRange(childrenCondition);
            return this;
        }

        public override object Clone()
        {
            var result =  base.Clone() as ConditionTree;
            result.Children = Children?.Select(x => x.Clone() as IConditionTree).ToList();
            result.AvailableChildren = AvailableChildren?.Select(x => x.Clone() as IConditionTree).ToList();
            return result;
        }

        public virtual void MergeFromPrototype(IConditionTree prototype)
        {
            if(prototype == null)
            {
                throw new ArgumentNullException(nameof(prototype));
            }

            void mergeFromPrototype(IEnumerable<IConditionTree> sourceList, ICollection<IConditionTree> targetList)
            {
                foreach (var source in sourceList)
                {
                    var existTargets = targetList.Where(x => x.Id.EqualsInvariant(source.Id));
                    if (existTargets.Any())
                    {
                        foreach (var target in existTargets)
                        {
                            target.MergeFromPrototype(source);
                        }
                    }
                    else
                    {
                        targetList.Add(source);
                    }
                }
            }

            if (prototype.AvailableChildren != null)
            {
                mergeFromPrototype(prototype.AvailableChildren, AvailableChildren);
            }
            if (prototype.Children != null)
            {
                mergeFromPrototype(prototype.Children, Children);              
            }
        }

        public virtual bool IsSatisfiedBy(IEvaluationContext context)
        {
            return true;
        }
    }
}
