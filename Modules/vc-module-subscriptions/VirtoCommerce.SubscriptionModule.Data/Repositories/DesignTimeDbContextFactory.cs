using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace VirtoCommerce.SubscriptionModule.Data.Repositories
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<SubscriptionDbContext>
    {
        public SubscriptionDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<SubscriptionDbContext>();

            builder.UseSqlServer("Data Source=(local);Initial Catalog=VirtoCommerce3;Persist Security Info=True;User ID=virto;Password=virto;MultipleActiveResultSets=True;Connect Timeout=30");

            return new SubscriptionDbContext(builder.Options);
        }
    }
}
