using System.Collections.Generic;
using VirtoCommerce.CoreModule.Core.Conditions;

namespace VirtoCommerce.MarketingModule.Core.Model.Promotions.Conditions
{
    public class BlockCatalogCondition : BlockConditionAndOr
    {
        public override IEnumerable<IConditionTree> AvailableChildren
        {
            get
            {
                yield return new ConditionCategoryIs();
                yield return new ConditionCodeContains();
                yield return new ConditionCurrencyIs();
                yield return new ConditionEntryIs();
                yield return new ConditionInStockQuantity();
            }
        }
    }
}
