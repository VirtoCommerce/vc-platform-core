using System.Collections.Generic;
using VirtoCommerce.CoreModule.Core.Conditions;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.MarketingModule.Core.Model.Promotions.Conditions
{
    public class BlockCatalogCondition : BlockConditionAndOr
    {
        public override IEnumerable<IConditionTree> AvailableChildren
        {
            get
            {
                yield return AbstractTypeFactory<ConditionCategoryIs>.TryCreateInstance();
                yield return AbstractTypeFactory<ConditionCodeContains>.TryCreateInstance();
                yield return AbstractTypeFactory<ConditionCurrencyIs>.TryCreateInstance();
                yield return AbstractTypeFactory<ConditionEntryIs>.TryCreateInstance();
                yield return AbstractTypeFactory<ConditionInStockQuantity>.TryCreateInstance();
            }
        }
    }
}
