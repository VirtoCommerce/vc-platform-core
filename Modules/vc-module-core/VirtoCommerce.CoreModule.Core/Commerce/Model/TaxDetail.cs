using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CoreModule.Core.Commerce.Model
{
	public class TaxDetail : ValueObject
	{
		public decimal Rate { get; set; }
		public decimal Amount { get; set; }
		public string Name { get; set; }
	}
}
