using VirtoCommerce.CoreModule.Core.Conditions;

namespace VirtoCommerce.PricingModule.Core.Model.Conditions
{
    public class PriceConditionTree : BlockConditionAndOr
    {
        public PriceConditionTree()
        {
            All = true;
        }
    }
}
