using VirtoCommerce.CartModule.Core.Model;

namespace VirtoCommerce.CartModule.Core.Services
{
    public interface IShoppingCartTotalsCalculator
    {
        void CalculateTotals(ShoppingCart cart);
    }
}
