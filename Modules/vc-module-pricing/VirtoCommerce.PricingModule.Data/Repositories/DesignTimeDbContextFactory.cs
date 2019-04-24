using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace VirtoCommerce.PricingModule.Data.Repositories
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<PricingDbContext>
    {
        public PricingDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<PricingDbContext>();

            builder.UseSqlServer("Data Source=(local);Initial Catalog=VirtoCommerce3;Persist Security Info=True;User ID=virto;Password=virto;MultipleActiveResultSets=True;Connect Timeout=30");

            return new PricingDbContext(builder.Options);
        }
    }
}
