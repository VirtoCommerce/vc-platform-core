using VirtoCommerce.PricingModule.Core.Model.CommonExpressions;

namespace VirtoCommerce.PricingModule.Core.Services
{
	public class DefaultPricingExtensionManagerImpl : IPricingExtensionManager
	{
		#region IPricingExtensionManager Members
		public virtual ConditionExpressionTree ConditionExpressionTree { get; set; }
		#endregion
	}
}
