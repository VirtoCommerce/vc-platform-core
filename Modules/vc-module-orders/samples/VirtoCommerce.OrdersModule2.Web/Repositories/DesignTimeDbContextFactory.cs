using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace VirtoCommerce.OrdersModule2.Web.Repositories
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<Order2DbContext>
    {
        public Order2DbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<Order2DbContext>();

            builder.UseSqlServer("Data Source=(local);Initial Catalog=VirtoCommerce3;Persist Security Info=True;User ID=virto;Password=virto;MultipleActiveResultSets=True;Connect Timeout=30");

            return new Order2DbContext(builder.Options);
        }
    }
}
