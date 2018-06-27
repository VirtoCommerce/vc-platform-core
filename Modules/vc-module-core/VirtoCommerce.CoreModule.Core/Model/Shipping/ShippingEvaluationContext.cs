using VirtoCommerce.CoreModule.Common;
using VirtoCommerce.CoreModule.Model.Cart;

namespace VirtoCommerce.CoreModule.Core.Model.Shipping
{
	public class ShippingEvaluationContext : IEvaluationContext
	{
		public ShippingEvaluationContext(ShoppingCart shoppingCart)
		{
			ShoppingCart = shoppingCart;
		}

		public ShoppingCart ShoppingCart { get; set; }
	}
}
