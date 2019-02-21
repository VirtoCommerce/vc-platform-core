using System.Linq;
using VirtoCommerce.CoreModule.Core.Common;

namespace VirtoCommerce.MarketingModule.Core.Model.Promotions.Conditions
{
    public class BlockCustomerCondition : BlockConditionAndOr
    {
        public override Condition[] GetConditions()
        {
            return Children.OfType<Condition>().ToArray();
        }
    }
}
