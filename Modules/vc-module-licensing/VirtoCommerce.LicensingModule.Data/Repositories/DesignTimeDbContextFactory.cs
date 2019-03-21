using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace VirtoCommerce.LicensingModule.Data.Repositories
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<LicenseDbContext>
    {
        public LicenseDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<LicenseDbContext>();

            builder.UseSqlServer("Data Source=(local);Initial Catalog=VirtoCommerce3;Persist Security Info=True;User ID=virto;Password=virto;MultipleActiveResultSets=True;Connect Timeout=30");

            return new LicenseDbContext(builder.Options);
        }
    }
}
