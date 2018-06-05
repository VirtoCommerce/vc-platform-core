using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Infrastructure;
using VirtoCommerce.Platform.Data.Repositories;

namespace Module1.Data
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<PlatformDbContext>
    {
        public PlatformDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<PlatformDbContext>();

            builder.UseSqlServer("Data Source=(local);Initial Catalog=VirtoCommerce3.0;Persist Security Info=True;User ID=virto;Password=virto;MultipleActiveResultSets=True;Connect Timeout=30", b => b.MigrationsAssembly("Module1.Data"));
            builder.ReplaceService<IModelCustomizer, PlatformDbContextCustomizer>();
            return new PlatformDbContext(builder.Options);
        }
    }
}
