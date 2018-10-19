using VirtoCommerce.PricingModule.Core.Model.CommonExpressions;

namespace VirtoCommerce.PricingModule.Core.Services
{
	public interface IPricingExtensionManager
	{
		ConditionExpressionTree ConditionExpressionTree { get; set; }
	}
}
