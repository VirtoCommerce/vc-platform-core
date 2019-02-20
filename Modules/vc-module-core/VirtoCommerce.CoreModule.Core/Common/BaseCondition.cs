using System;
using System.Collections.Generic;
using System.Linq;

namespace VirtoCommerce.CoreModule.Core.Common
{
    public abstract class BaseCondition : ICondition
    {
        public BaseCondition()
        {
            Id = GetType().Name;
            AvailableChildren = new List<ICondition>();
            Children = new List<ICondition>();
        }

        public ICollection<ICondition> AvailableChildren { get; set; }
        public ICollection<ICondition> Children { get; set; }
        public string Id { get; set; }
        public virtual bool Evaluate(IEvaluationContext context)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ICondition> GetConditions()
        {
            return Children.OfType<ICondition>();
        }
    }
}
