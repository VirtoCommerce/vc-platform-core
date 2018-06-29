using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CoreModule.Core.Model.Cart
{
	public class PaymentMethod : ValueObject
	{
		public string GatewayCode { get; set; }
		public string Name { get; set; }
		public string IconUrl { get; set; }
	}
}
