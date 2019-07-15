using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.OrdersModule.Core.Model;
using VirtoCommerce.OrdersModule.Data.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.OrdersModule.Data.Repositories
{
    public interface IOrderRepository : IRepository
    {
        IQueryable<CustomerOrderEntity> CustomerOrders { get; }
        IQueryable<ShipmentEntity> Shipments { get; }
        IQueryable<PaymentInEntity> InPayments { get; }
        IQueryable<AddressEntity> Addresses { get; }
        IQueryable<LineItemEntity> LineItems { get; }

        Task<CustomerOrderEntity[]> GetCustomerOrdersByIdsAsync(string[] ids, string responseGroup = null);
        Task RemoveOrdersByIdsAsync(string[] ids);
    }
}
