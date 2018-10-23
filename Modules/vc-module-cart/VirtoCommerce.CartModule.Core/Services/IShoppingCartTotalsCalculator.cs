using VirtoCommerce.CartModule.Core.Model;

namespace VirtoCommerce.CartModule.Core.Services
{
    public interface IShopingCartTotalsCalculator
    {
        void CalculateTotals(ShoppingCart cart);
    }
}
