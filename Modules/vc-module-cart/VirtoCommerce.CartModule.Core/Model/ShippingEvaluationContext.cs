using VirtoCommerce.ShippingModule.Core.Model;

namespace VirtoCommerce.CartModule.Core.Model
{
    public class ShippingEvaluationContext : ShippingRateEvaluationContext
    {
        public ShippingEvaluationContext(ShoppingCart shoppingCart)
        {
            ShoppingCart = shoppingCart;
            Currency = shoppingCart.Currency;
        }

        public ShoppingCart ShoppingCart { get; }
    }
}
