using VirtoCommerce.CoreModule.Core.Common.Conditions;

namespace VirtoCommerce.PricingModule.Core.Services
{
    public class DefaultPricingExtensionManagerImpl : IPricingExtensionManager
    {
        #region IPricingExtensionManager Members
        public virtual IConditionTree PriceConditionTree { get; set; }
        #endregion
    }
}
