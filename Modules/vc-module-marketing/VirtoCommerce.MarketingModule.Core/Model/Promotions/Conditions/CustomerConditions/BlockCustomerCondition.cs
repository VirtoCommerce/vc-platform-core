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
                yield return new ConditionIsRegisteredUser();
                yield return new ConditionIsEveryone();
                yield return new ConditionIsFirstTimeBuyer();
                yield return new UserGroupsContainsCondition();
            }
        }
    }
}
