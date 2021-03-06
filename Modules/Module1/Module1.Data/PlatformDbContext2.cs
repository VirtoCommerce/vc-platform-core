using Microsoft.EntityFrameworkCore;
using VirtoCommerce.Platform.Data.Repositories;

namespace Module1.Data
{
    public class PlatformDbContext2 : PlatformDbContext
    {
        public PlatformDbContext2(DbContextOptions<PlatformDbContext2> options) : base(options)
        {
        }

        protected PlatformDbContext2(DbContextOptions options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


        }
    }
}
