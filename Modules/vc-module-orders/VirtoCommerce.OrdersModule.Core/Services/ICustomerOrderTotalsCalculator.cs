using VirtoCommerce.OrdersModule.Core.Model;

namespace VirtoCommerce.OrdersModule.Core.Services
{
    public interface ICustomerOrderTotalsCalculator
    {
        void CalculateTotals(CustomerOrder order);
    }
}
