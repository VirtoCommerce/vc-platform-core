using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CartModule.Core.Model
{
	public class PaymentMethod : ValueObject
	{
		public string GatewayCode { get; set; }
		public string Name { get; set; }
		public string IconUrl { get; set; }
	}
}
