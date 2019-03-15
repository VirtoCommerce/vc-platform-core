using System.Linq;
using VirtoCommerce.CoreModule.Core.Conditions;

namespace VirtoCommerce.MarketingModule.Core.Model.Promotions.Conditions
{
    public class BlockCatalogCondition : BlockConditionAndOr
    {
        public override Condition[] GetConditions()
        {
            return Children.OfType<Condition>().ToArray();
        }
    }
}
