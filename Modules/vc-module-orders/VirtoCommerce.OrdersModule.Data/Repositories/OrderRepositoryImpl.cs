using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VirtoCommerce.OrdersModule.Core.Model;
using VirtoCommerce.OrdersModule.Data.Model;
using VirtoCommerce.Platform.Core.Domain;
using VirtoCommerce.Platform.Data.Infrastructure;

namespace VirtoCommerce.OrdersModule.Data.Repositories
{
    public class OrderRepositoryImpl : DbContextRepositoryBase<OrderDbContext>, IOrderRepository
    {
        public OrderRepositoryImpl(OrderDbContext dbContext, IUnitOfWork unitOfWork = null) : base(dbContext, unitOfWork)
        {
        }

        public IQueryable<CustomerOrderEntity> CustomerOrders => DbContext.Set<CustomerOrderEntity>();
        public IQueryable<ShipmentEntity> Shipments => DbContext.Set<ShipmentEntity>();
        public IQueryable<ShipmentPackageEntity> ShipmentPackagesPackages => DbContext.Set<ShipmentPackageEntity>();
        public IQueryable<ShipmentItemEntity> ShipmentItems => DbContext.Set<ShipmentItemEntity>();
        public IQueryable<DiscountEntity> Discounts => DbContext.Set<DiscountEntity>();
        public IQueryable<TaxDetailEntity> TaxDetails => DbContext.Set<TaxDetailEntity>();
        public IQueryable<PaymentInEntity> InPayments => DbContext.Set<PaymentInEntity>();
        public IQueryable<AddressEntity> Addresses => DbContext.Set<AddressEntity>();
        public IQueryable<LineItemEntity> LineItems => DbContext.Set<LineItemEntity>();
        public IQueryable<PaymentGatewayTransactionEntity> Transactions => DbContext.Set<PaymentGatewayTransactionEntity>();

        public virtual async Task<CustomerOrderEntity[]> GetCustomerOrdersByIdsAsync(string[] ids, CustomerOrderResponseGroup responseGroup)
        {
            var result = await CustomerOrders.Where(x => ids.Contains(x.Id)).ToArrayAsync();
            var orderDiscounts = Discounts.Where(x => ids.Contains(x.CustomerOrderId)).ToArrayAsync();
            var orderTaxDetails = TaxDetails.Where(x => ids.Contains(x.CustomerOrderId)).ToArrayAsync();
            await Task.WhenAll(orderDiscounts, orderTaxDetails);

            if ((responseGroup & CustomerOrderResponseGroup.WithAddresses) == CustomerOrderResponseGroup.WithAddresses)
            {
                var addresses = await Addresses.Where(x => ids.Contains(x.CustomerOrderId)).ToArrayAsync();
            }

            if ((responseGroup & CustomerOrderResponseGroup.WithInPayments) == CustomerOrderResponseGroup.WithInPayments)
            {
                var inPayments = await InPayments.Where(x => ids.Contains(x.CustomerOrderId)).ToArrayAsync();
                var paymentsIds = inPayments.Select(x => x.Id).ToArray();
                var paymentDiscounts = Discounts.Where(x => paymentsIds.Contains(x.PaymentInId)).ToArrayAsync();
                var paymentTaxDetails = TaxDetails.Where(x => paymentsIds.Contains(x.PaymentInId)).ToArrayAsync();
                var paymentAddresses = Addresses.Where(x => paymentsIds.Contains(x.PaymentInId)).ToArrayAsync();
                var transactions = Transactions.Where(x => paymentsIds.Contains(x.PaymentInId)).ToArrayAsync();
                await Task.WhenAll(paymentDiscounts, paymentTaxDetails, paymentAddresses, transactions);
            }

            if ((responseGroup & CustomerOrderResponseGroup.WithItems) == CustomerOrderResponseGroup.WithItems)
            {
                var lineItems = await LineItems.Where(x => ids.Contains(x.CustomerOrderId))
                                         .OrderByDescending(x => x.CreatedDate).ToArrayAsync();
                var lineItemIds = lineItems.Select(x => x.Id).ToArray();
                var lineItemDiscounts = Discounts.Where(x => lineItemIds.Contains(x.LineItemId)).ToArrayAsync();
                var lineItemTaxDetails = TaxDetails.Where(x => lineItemIds.Contains(x.LineItemId)).ToArrayAsync();
                await Task.WhenAll(lineItemDiscounts, lineItemTaxDetails);
            }

            if ((responseGroup & CustomerOrderResponseGroup.WithShipments) == CustomerOrderResponseGroup.WithShipments)
            {
                var shipments = await Shipments.Where(x => ids.Contains(x.CustomerOrderId)).ToArrayAsync();
                var shipmentIds = shipments.Select(x => x.Id).ToArray();
                var shipmentDiscounts = Discounts.Where(x => shipmentIds.Contains(x.ShipmentId)).ToArrayAsync();
                var shipmentTaxDetails = TaxDetails.Where(x => shipmentIds.Contains(x.ShipmentId)).ToArrayAsync();
                var addresses = Addresses.Where(x => shipmentIds.Contains(x.ShipmentId)).ToArrayAsync();
                var shipmentItems = ShipmentItems.Where(x => shipmentIds.Contains(x.ShipmentId)).ToArrayAsync();
                var packages = ShipmentPackagesPackages.Include(x => x.Items).Where(x => shipmentIds.Contains(x.ShipmentId)).ToArrayAsync();
                await Task.WhenAll(shipmentDiscounts, shipmentTaxDetails, addresses, shipmentItems, packages);
            }
            return result;
        }

        public virtual async Task RemoveOrdersByIdsAsync(string[] ids)
        {
            var orders = await GetCustomerOrdersByIdsAsync(ids, CustomerOrderResponseGroup.Full);
            foreach (var order in orders)
            {
                Remove(order);
            }
        }

        
    }
}
