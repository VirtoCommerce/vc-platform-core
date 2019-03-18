using VirtoCommerce.CoreModule.Core.Common.Conditions;

namespace VirtoCommerce.PricingModule.Core.Services
{
    public interface IPricingExtensionManager
    {
        IConditionTree PriceConditionTree { get; set; }
    }
}
