using System.Collections.Generic;
using VirtoCommerce.CoreModule.Core.Conditions;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.MarketingModule.Core.Model.Promotions.Conditions
{
    public class BlockCustomerCondition : BlockConditionAndOr
    {
        public override IEnumerable<IConditionTree> AvailableChildren
        {
            get
            {
                yield return AbstractTypeFactory<ConditionIsRegisteredUser>.TryCreateInstance();
                yield return AbstractTypeFactory<ConditionIsEveryone>.TryCreateInstance();
                yield return AbstractTypeFactory<ConditionIsFirstTimeBuyer>.TryCreateInstance();
                yield return AbstractTypeFactory<UserGroupsContainsCondition>.TryCreateInstance();
            }
        }
    }
}
