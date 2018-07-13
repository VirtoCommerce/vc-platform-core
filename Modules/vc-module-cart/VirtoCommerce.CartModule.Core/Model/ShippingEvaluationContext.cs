using VirtoCommerce.CoreModule.Core.Common;

namespace VirtoCommerce.CartModule.Core.Model
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
