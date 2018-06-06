using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Module1.Data
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<PlatformDbContext2>
    {
        public PlatformDbContext2 CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<PlatformDbContext2>();

            builder.UseSqlServer("Data Source=(local);Initial Catalog=VirtoCommerce3.0;Persist Security Info=True;User ID=virto;Password=virto;MultipleActiveResultSets=True;Connect Timeout=30");
            return new PlatformDbContext2(builder.Options);
        }
    }
}
