using Microsoft.EntityFrameworkCore;
using VirtoCommerce.Platform.Data.Repositories;

namespace Module1.Data
{
    public class PlatformDbContext2 : PlatformDbContext
    {
        public PlatformDbContext2(DbContextOptions<PlatformDbContext2> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<SettingEntity2>(entity =>
            {
                entity.ToTable("PlatformSetting");
            });
        }
    }
}
