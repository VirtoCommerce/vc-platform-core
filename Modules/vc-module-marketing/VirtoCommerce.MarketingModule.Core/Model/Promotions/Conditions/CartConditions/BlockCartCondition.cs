using System.Collections.Generic;
using VirtoCommerce.CoreModule.Core.Conditions;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.MarketingModule.Core.Model.Promotions.Conditions
{
    public class BlockCartCondition : BlockConditionAndOr
    {
        public override IEnumerable<IConditionTree> AvailableChildren
        {
            get
            {
                yield return AbstractTypeFactory<ConditionAtNumItemsInCart>.TryCreateInstance();
                yield return AbstractTypeFactory<ConditionAtNumItemsInCategoryAreInCart>.TryCreateInstance();
                yield return AbstractTypeFactory<ConditionAtNumItemsOfEntryAreInCart>.TryCreateInstance();
                yield return AbstractTypeFactory<ConditionCartSubtotalLeast>.TryCreateInstance();             
            }
        }
    }
}
