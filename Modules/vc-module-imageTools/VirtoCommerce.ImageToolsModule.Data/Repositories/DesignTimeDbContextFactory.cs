using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace VirtoCommerce.ImageToolsModule.Data.Repositories
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ThumbnailDbContext>
    {
        public ThumbnailDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<ThumbnailDbContext>();

            builder.UseSqlServer("Data Source=(local);Initial Catalog=VirtoCommerce3;Persist Security Info=True;User ID=virto;Password=virto;MultipleActiveResultSets=True;Connect Timeout=30");

            return new ThumbnailDbContext(builder.Options);
        }
    }
}
