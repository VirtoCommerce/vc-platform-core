using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace VirtoCommerce.TaxModule.Data.Repositories
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<TaxDbContext>
    {
        public TaxDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<TaxDbContext>();

            builder.UseSqlServer("Data Source=(local);Initial Catalog=VirtoCommerce3;Persist Security Info=True;User ID=virto;Password=virto;MultipleActiveResultSets=True;Connect Timeout=30");

            return new TaxDbContext(builder.Options);
        }
    }
}
