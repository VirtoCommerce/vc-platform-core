using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace VirtoCommerce.MarketingModule.Data.Repositories
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<MarketingDbContext>
    {
        public MarketingDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<MarketingDbContext>();

            builder.UseSqlServer("Data Source=(local);Initial Catalog=VirtoCommerce3;Persist Security Info=True;User ID=virto;Password=virto;MultipleActiveResultSets=True;Connect Timeout=30");

            return new MarketingDbContext(builder.Options);
        }
    }
}
