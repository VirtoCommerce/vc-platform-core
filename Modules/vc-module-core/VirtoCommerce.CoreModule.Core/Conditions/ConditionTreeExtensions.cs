using System;
using System.Linq;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CoreModule.Core.Conditions
{
    public static class ConditionTreeExtensions
    {
        public static void EnableAvailableChildrenSerialization(this IConditionTree conditionTree)
        {
            if(conditionTree == null)
            {
                throw new ArgumentNullException(nameof(conditionTree));
            }
            foreach (var node in conditionTree.GetFlatObjectsListWithInterface<IConditionTree>().OfType<ConditionTree>())
            {
                node.ShouldSerializeAvailableChildrenFlag = true;
            }

        }
    }
}
