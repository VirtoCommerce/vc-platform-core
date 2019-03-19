using VirtoCommerce.CoreModule.Core.Conditions;

namespace VirtoCommerce.PricingModule.Core.Services
{
    public interface IPricingExtensionManager
    {
        IConditionTree PriceConditionTree { get; set; }
    }
}
