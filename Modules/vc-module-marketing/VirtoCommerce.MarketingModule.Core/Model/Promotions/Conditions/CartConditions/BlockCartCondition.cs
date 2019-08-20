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
                yield return new ConditionAtNumItemsInCart();
                yield return new ConditionAtNumItemsInCategoryAreInCart();
                yield return new ConditionAtNumItemsOfEntryAreInCart();
                yield return new ConditionCartSubtotalLeast();             
            }
        }
    }
}
