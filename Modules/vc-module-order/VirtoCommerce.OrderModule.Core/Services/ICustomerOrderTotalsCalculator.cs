using VirtoCommerce.OrderModule.Core.Model;

namespace VirtoCommerce.OrderModule.Core.Services
{
    public interface ICustomerOrderTotalsCalculator
    {
        void CalculateTotals(CustomerOrder order);
    }
}
