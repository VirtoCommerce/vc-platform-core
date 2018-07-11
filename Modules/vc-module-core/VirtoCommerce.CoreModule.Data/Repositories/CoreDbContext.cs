using EntityFrameworkCore.Triggers;
using Microsoft.EntityFrameworkCore;
using VirtoCommerce.CoreModule.Data.Model;

namespace VirtoCommerce.CoreModule.Data.Repositories
{
    public class CoreDbContext : DbContextWithTriggers
    {
        public CoreDbContext(DbContextOptions<CoreDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SeoUrlKeywordEntity>().ToTable("SeoUrlKeyword").HasKey(x => x.Id);
            modelBuilder.Entity<SeoUrlKeywordEntity>().Property(x => x.Id).HasMaxLength(128);
            modelBuilder.Entity<SeoUrlKeywordEntity>().HasIndex(x => new { x.Keyword, x.StoreId }).HasName("IX_KeywordStoreId");
            modelBuilder.Entity<SeoUrlKeywordEntity>().HasIndex(x => new { x.ObjectId, x.ObjectType }).HasName("IX_ObjectIdAndObjectType");

            modelBuilder.Entity<SequenceEntity>().ToTable("Sequence").HasKey(x => x.ObjectType);

            modelBuilder.Entity<CurrencyEntity>().ToTable("Currency").HasKey(x => x.Id);
            modelBuilder.Entity<CurrencyEntity>().Property(x => x.Id).HasMaxLength(128);
            modelBuilder.Entity<CurrencyEntity>().HasIndex(x => x.Code).HasName("IX_Code");

            modelBuilder.Entity<PackageTypeEntity>().ToTable("PackageType").HasKey(x => x.Id);
            modelBuilder.Entity<PackageTypeEntity>().Property(x => x.Id).HasMaxLength(128);

            base.OnModelCreating(modelBuilder);
        }
    }
}
