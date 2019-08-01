using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.OrdersModule.Core.Model;
using VirtoCommerce.OrdersModule.Data.Model;
using VirtoCommerce.OrdersModule.Data.Repositories;
using VirtoCommerce.OrdersModule2.Web.Model;
using VirtoCommerce.Platform.Core.Domain;

namespace VirtoCommerce.OrdersModule2.Web.Repositories
{
    public class OrderRepository2 : OrderRepository
    {
        public OrderRepository2(Order2DbContext dbContext, IUnitOfWork unitOfWork = null) : base(dbContext, unitOfWork)
        {
        }

        public IQueryable<CustomerOrder2Entity> CustomerOrders2 => DbContext.Set<CustomerOrder2Entity>();
        public IQueryable<InvoiceEntity> Invoices => DbContext.Set<InvoiceEntity>();

        public override async Task<CustomerOrderEntity[]> GetCustomerOrdersByIdsAsync(string[] ids, string responseGroup = null)
        {
            var retVal = await base.GetCustomerOrdersByIdsAsync(ids, responseGroup);
            var invoices = Invoices.Where(x => ids.Contains(x.CustomerOrder2Id)).ToArray();
            return retVal;
        }
    }
}
