using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace VirtoCommerce.CustomerSampleModule.Web.Repositories
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<CustomerSampleDbContext>
    {
        public CustomerSampleDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<CustomerSampleDbContext>();

            builder.UseSqlServer("Data Source=(local);Initial Catalog=VirtoCommerce3;Persist Security Info=True;User ID=virto;Password=virto;MultipleActiveResultSets=True;Connect Timeout=30");

            return new CustomerSampleDbContext(builder.Options);
        }
    }
}
